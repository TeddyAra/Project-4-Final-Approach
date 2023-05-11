using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using GXPEngine.Core;
using SharpDX.DirectInput;
using SharpDX.XInput;

public class Player : EasyDraw {
    Sprite body;
    Sprite tongue;
    List<Sprite> tongues;
    Sprite head;

    MyGame myGame;
    Line line;
    List<Line> bodyLines;
    List<Line> lines;
    bool againstSolid;
    bool holdingItem;
    Vec2 tongueTip;
    Vec2 oldTongueTip;
    Vec2 tongueDirectionVec;
    Item heldItem;

    int bodyHeightHalf; // Y position of the origin point of the body
    int tongueNumber; // How many pieces the tongue should be made out of

    Vec2 pos; // The body's position
    Vec2 vel; // The body's movement velocity
    Vec2 force; // The force applied on the body
    int mass; // The body's mass
    float rotVel; // The body's rotation velocity
    int headAngle; // The angle of the head and the tongue
    int headRotSpeed; // The speed that the head rotates with
    int headMaxAngle; // The max angle the head can differ from looking straight
    int tongueLength; // The length of the tongue that's sticking out
    int tongueSpeed; // How fast the tongue moves
    int grabCooldown;

    Joystick joystick;
    JoystickState state;

    public Player(Guid joystickGuid, List<Line> lines) : base(2, 2) {
        myGame = (MyGame)game;
        this.lines = lines;

        headMaxAngle = 45;
        tongueNumber = 10;
        tongueLength = 200;

        pos = new Vec2(game.width / 2, 200);

        SetupBody();

        joystick = new Joystick(new DirectInput(), joystickGuid);
        joystick.Properties.BufferSize = 128;
        joystick.Properties.DeadZone = 2000;
    }

    void Update() {
        pos += vel;
        SetXY(pos.x, pos.y);
        grabCooldown--;

        joystick.Acquire();
        state = joystick.GetCurrentState();

        if (state.Buttons[6]) {
            rotation--;
        }

        if (state.Buttons[7]) {
            rotation++;
        }

        // Head controls
        if (state.Buttons[4] && head.rotation > -headMaxAngle) {
            head.rotation -= 2;
            if (holdingItem)
                heldItem.SetXY(tongueTip.x, tongueTip.y);
            Line line = new Line(tongueTip.x, tongueTip.y, head.TransformPoint(0, 0).x, head.TransformPoint(0, 0).y);
            for (int i = 0; i < lines.Count(); i++) {
                if (lines[i].CheckIntersection(line)) {
                    head.rotation++;
                }
            }
        }

        if (state.Buttons[5] && head.rotation < headMaxAngle) {
            head.rotation += 2;
            if (holdingItem)
                heldItem.SetXY(tongueTip.x, tongueTip.y);
            Line line = new Line(tongueTip.x, tongueTip.y, head.TransformPoint(0, 0).x, head.TransformPoint(0, 0).y);
            for (int i = 0; i < lines.Count(); i++) {
                if (lines[i].CheckIntersection(line)) {
                    head.rotation--;
                }
            }
        }

        // Tongue controls
        tongueSpeed = 3;

        if (state.Buttons[1] && tongues[0].y > 0 && !againstSolid) {
            tongueLength -= tongueSpeed;
            if (holdingItem)
                heldItem.SetXY(tongueTip.x, tongueTip.y);
        }

        if (state.Buttons[2] && tongues[0].y < (tongues.Count() - 1) * (tongues[0].height - tongues[0].width / 2)) {
            tongueLength += tongueSpeed;
            if (holdingItem) {
                heldItem.vel = tongueTip - oldTongueTip;
                holdingItem = false;
                grabCooldown = 60;
            }
        }

        if (state.Buttons[0] && tongues[0].y < (tongues.Count() - 1) * (tongues[0].height - tongues[0].width / 2)) {
            if (againstSolid) {
                pos += Vec2.GetUnitVectorDeg(head.rotation - 90 + rotation) * tongueSpeed;
                Console.WriteLine(Vec2.GetUnitVectorDeg(head.rotation - 90 + rotation) * tongueSpeed);
                tongueLength += tongueSpeed;
            } else if (holdingItem) {
                tongueLength += tongueSpeed;
            }
        }

        if (holdingItem) {
            heldItem.pos = new Vec2(tongueTip.x, tongueTip.y);
        }

        tongues[0].SetXY(0, tongueLength);
        for (int i = 0; i < tongues.Count(); i++) {
            if (tongues[0].y - (i + 0.5f) * (tongues[0].height - tongues[0].width / 2) > 0) {
                tongues[i].alpha = 0;
            } else {
                tongues[i].alpha = 1;
            }
        }

        // Check for collisions
        tongueDirectionVec = new Vec2(tongues[tongues.Count() - 1].TransformDirection(1, 0).x, tongues[tongues.Count() - 1].TransformDirection(1, 0).y).RotatedDeg(-90);
        oldTongueTip = tongueTip;
        tongueTip = new Vec2(tongues[tongues.Count() - 1].TransformPoint(0, 0).x, tongues[tongues.Count() - 1].TransformPoint(0, 0).y) + tongueDirectionVec * tongues[0].height;

        againstSolid = false;

        for (int i = 0; i < lines.Count(); i++) {
            float projection = (lines[i].point2 - lines[i].point1).Normalized().Dot(tongueTip - new Vec2(lines[i].TransformPoint(0, 0).x + lines[i].point1.x, lines[i].TransformPoint(0, 0).y + lines[i].point1.y));
            Vec2 impactPoint = new Vec2(lines[i].TransformPoint(0, 0).x + lines[i].point1.x, lines[i].TransformPoint(0, 0).y + lines[i].point1.y) + (lines[i].point2 - lines[i].point1).Normalized() * projection;

            if (projection > 0 && projection < (lines[i].point1 - lines[i].point2).Length() && (impactPoint - tongueTip).Length() < 5 && grabCooldown < 0) {
                if (lines[i].item != null && !holdingItem) {
                    holdingItem = true;
                    heldItem = lines[i].item;
                } else {
                    againstSolid = true;
                    vel = new Vec2(0, 0);
                }
            }
        }
    }

    void SetupBody() { 
        bodyHeightHalf = 25;

        SetOrigin(width / 2, height / 2);

        body = new Sprite("body.png");
        AddChild(body);
        body.SetOrigin(body.width / 2, bodyHeightHalf);
        body.SetXY(width / 2, height / 2);

        head = new Sprite("head.png");

        tongues = new List<Sprite>();
        tongue = new Sprite("tongue.png");
        head.AddChild(tongue);
        tongue.SetOrigin(tongue.width / 2, tongue.height);
        tongue.SetXY(0, -bodyHeightHalf);
        tongues.Add(tongue);

        for (int i = 0; i < tongueNumber; i++) {
            tongue = new Sprite("tongue.png");
            tongues[tongues.Count() - 1].AddChild(tongue);
            tongue.SetOrigin(tongue.width / 2, tongue.height);
            tongue.SetXY(0, -tongue.height + tongue.width / 2);
            tongues.Add(tongue);
        }

        body.AddChild(head);
        head.SetOrigin(head.width / 2, head.height / 2);
        head.SetXY(0, -bodyHeightHalf);
    }
}