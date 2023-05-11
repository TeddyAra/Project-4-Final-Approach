using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using GXPEngine.Core;
using SharpDX.DirectInput;
using SharpDX.XInput;

public class Player : EasyDraw {
    Sprite rightUpperArm;
    Sprite leftUpperArm;
    Sprite body;
    Sprite rightLowerArm;
    Sprite leftLowerArm;
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
    int shoulderDistance; // How much the Y positions of the shoulders are different from the minimum

    Vec2 pos; // The body's position
    Vec2 vel; // The body's movement velocity
    Vec2 force; // The force applied on the body
    int mass; // The body's mass
    float rotVel; // The body's rotation velocity
    public Vec2 rightHandPos; // The position of the right hand
    public Vec2 leftHandPos; // The position of the left hand
    Vec2 oldRightHandPos; // The old position of the right hand
    Vec2 oldLeftHandPos; // The old position of the left hand
    Vec2 rightHandVel; // The velocity of the right hand
    Vec2 leftHandVel; // The velocity of the left hand
    int handMoveSpeed; // The max speed of the hand movement
    int headAngle; // The angle of the head and the tongue
    int headRotSpeed; // The speed that the head rotates with
    int armMaxDistance; // The max distance the hand positions can be from the shoulders
    int headMaxAngle; // The max angle the head can differ from looking straight
    int tongueLength; // The length of the tongue that's sticking out
    int tongueSpeed; // How fast the tongue moves

    Joystick joystick;
    JoystickState state;
    VerletBody leftArm;
    VerletBody rightArm;

    public Player(Guid joystickGuid, List<Line> lines) : base(2, 2) {
        myGame = (MyGame)game;
        this.lines = lines;

        handMoveSpeed = 10;
        armMaxDistance = 75;
        headMaxAngle = 45;
        tongueNumber = 10;
        tongueLength = 200;

        pos = new Vec2(game.width / 2, 200);
        leftHandPos = pos - new Vec2(60, 0);
        rightHandPos = pos + new Vec2(60, 0);

        SetupBody();

        joystick = new Joystick(new DirectInput(), joystickGuid);
        joystick.Properties.BufferSize = 128;
        joystick.Properties.DeadZone = 2000;

        leftArm = new VerletBody();
        rightArm = new VerletBody();

        for (int i = 0; i < 3; i++) {
            leftArm.AddPoint(new VerletPoint(width / 2, 100 + i * 50, i == 0));
            rightArm.AddPoint(new VerletPoint(width / 2, 100 + i * 50, i == 0));
            if (i > 0) {
                leftArm.AddConstraint(i - 1, i, 1, true);
                rightArm.AddConstraint(i - 1, i, 1, true);
            }
        }
    }

    void Update() {
        pos += vel;
        SetXY(pos.x, pos.y);

        joystick.Acquire();
        state = joystick.GetCurrentState();

        if (state.Buttons[6]) {
            rotation--;
        }

        if (state.Buttons[7]) {
            rotation++;
        }

        // Updates right hand position
        rightHandVel = new Vec2(state.Z - 65535 / 2, state.RotationZ - 65535 / 2);
        oldRightHandPos = rightHandPos;
        rightHandPos += rightHandVel / 65535 * handMoveSpeed;

        // Keeps right hand within bounds
        if ((pos + new Vec2(rightUpperArm.x, rightUpperArm.y) - rightHandPos).RotatedDeg(-rotation).x > -body.width / 2) {
            do {
                rightHandPos += new Vec2(1, 0).RotatedDeg(rotation);
            } while ((pos + new Vec2(rightUpperArm.x, rightUpperArm.y) - rightHandPos).RotatedDeg(-rotation).x > -body.width / 2);
        }

        if ((pos + new Vec2(rightUpperArm.x, rightUpperArm.y) - rightHandPos).Length() > armMaxDistance) {
            do {
                rightHandPos += (pos + new Vec2(rightUpperArm.x, rightUpperArm.y) - rightHandPos).Normalized();
            } while ((pos + new Vec2(rightUpperArm.x, rightUpperArm.y) - rightHandPos).Length() > armMaxDistance);
        }

        // Updates left hand position
        leftHandVel = new Vec2(state.X - 65535 / 2, state.Y - 65535 / 2);
        oldLeftHandPos = leftHandPos;
        leftHandPos += leftHandVel / 65535 * handMoveSpeed;

        // Keeps left hand within bounds
        if ((pos + new Vec2(leftUpperArm.x, leftUpperArm.y) - leftHandPos).RotatedDeg(-rotation).x < body.width / 2) {
            do {
                leftHandPos += new Vec2(-1, 0).RotatedDeg(rotation);
            } while ((pos + new Vec2(leftUpperArm.x, leftUpperArm.y) - leftHandPos).RotatedDeg(-rotation).x < body.width / 2);
        }

        if ((pos + new Vec2(leftUpperArm.x, leftUpperArm.y) - leftHandPos).Length() > armMaxDistance) {
            do {
                leftHandPos += (pos + new Vec2(leftUpperArm.x, leftUpperArm.y) - leftHandPos).Normalized();
            } while ((pos + new Vec2(leftUpperArm.x, leftUpperArm.y) - leftHandPos).Length() > armMaxDistance);
        }

        // Rotates arms
        /*leftArm.UpdateVerlet();
        leftArm.UpdateConstraints();
        rightArm.UpdateVerlet();
        rightArm.UpdateConstraints();*/

        /*Vec2 leftShoulderPos = new Vec2(leftUpperArm.TransformPoint(0, 0).x, leftUpperArm.TransformPoint(0, 0).y);
        Vec2 difference = leftHandPos - leftShoulderPos;
        Vec2 elbowMidpoint = leftShoulderPos + difference / 2;
        float distance = Mathf.Sqrt(Mathf.Pow(leftUpperArm.width, 2) - Mathf.Pow(elbowMidpoint.Length(), 2));
        Vec2 elbow = elbowMidpoint + difference.Normal() * distance;
        float angle1 = (elbow - leftShoulderPos).GetAngleDeg();
        float angle2 = (leftShoulderPos - leftHandPos).GetAngleDeg();

        Console.WriteLine($"Shoulder: {leftShoulderPos}");

        leftUpperArm.rotation = angle1;
        leftLowerArm.rotation = angle2;*/

        // Head controls
        if (state.Buttons[4] && head.rotation > -headMaxAngle) {
            head.rotation--;
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
            head.rotation++;
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
            holdingItem = false;
        }

        if (state.Buttons[0] && tongues[0].y < (tongues.Count() - 1) * (tongues[0].height - tongues[0].width / 2)) {
            if (againstSolid) {
                pos += Vec2.GetUnitVectorDeg(head.rotation - 90 + rotation) * tongueSpeed;
                Console.WriteLine(Vec2.GetUnitVectorDeg(head.rotation - 90 + rotation) * tongueSpeed);
                tongueLength += tongueSpeed;
            } else if (holdingItem) {
                tongueLength += tongueSpeed;
                heldItem.SetXY(tongueTip.x, tongueTip.y);
            }
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

            if (projection > 0 && projection < (lines[i].point1 - lines[i].point2).Length() && (impactPoint - tongueTip).Length() < 5) {
                if (lines[i].item != null && !holdingItem) {
                    holdingItem = true;
                    heldItem = lines[i].item;
                } else {
                    againstSolid = true;
                    vel = new Vec2(0, 0);
                }
            }

            if (lines[i].solid != null) {
                projection = (lines[i].point2 - lines[i].point1).Normalized().Dot(leftHandPos - new Vec2(lines[i].TransformPoint(0, 0).x + lines[i].point1.x, lines[i].TransformPoint(0, 0).y + lines[i].point1.y));
                Vec2 point = lines[i].point1 + (lines[i].point2 - lines[i].point1).Normalized() * projection;
                float distance = (leftHandPos - (point + new Vec2(lines[i].solid.x, lines[i].solid.y))).Length();
                if (projection > 0 && projection < (lines[i].point1 - lines[i].point2).Length() && distance < 5) {
                    force = oldLeftHandPos - leftHandPos;
                    vel += force;
                    pos += vel;
                }

                projection = (lines[i].point2 - lines[i].point1).Normalized().Dot(rightHandPos - new Vec2(lines[i].TransformPoint(0, 0).x + lines[i].point1.x, lines[i].TransformPoint(0, 0).y + lines[i].point1.y));
                point = lines[i].point1 + (lines[i].point2 - lines[i].point1).Normalized() * projection;
                distance = (rightHandPos - (point + new Vec2(lines[i].solid.x, lines[i].solid.y))).Length();
                if (projection > 0 && projection < (lines[i].point1 - lines[i].point2).Length() && distance < 5) {
                    force = oldRightHandPos - rightHandPos;
                    vel += force;
                    pos += vel;
                }
            }
        }
    }

    void SetupBody() { 
        shoulderDistance = 0;
        bodyHeightHalf = 25;

        SetOrigin(width / 2, height / 2);

        body = new Sprite("body.png");
        AddChild(body);
        body.SetOrigin(body.width / 2, bodyHeightHalf);
        body.SetXY(width / 2, height / 2);

        rightUpperArm = new Sprite("right_upper_arm.png");
        body.AddChild(rightUpperArm);
        rightUpperArm.SetOrigin(rightUpperArm.height / 2, rightUpperArm.height / 2);
        rightUpperArm.SetXY(body.width / 2, -bodyHeightHalf + rightUpperArm.height / 2 + shoulderDistance);
        leftUpperArm = new Sprite("left_upper_arm.png");
        body.AddChild(leftUpperArm);
        leftUpperArm.SetOrigin(leftUpperArm.width - leftUpperArm.height / 2, leftUpperArm.height / 2);
        leftUpperArm.SetXY(-body.width / 2, -bodyHeightHalf + leftUpperArm.height / 2 + shoulderDistance);

        rightLowerArm = new Sprite("right_lower_arm.png");
        rightUpperArm.AddChild(rightLowerArm);
        rightLowerArm.SetOrigin(rightLowerArm.height / 2, rightLowerArm.height / 2);
        rightLowerArm.SetXY(rightUpperArm.width - rightLowerArm.height / 2, 0);
        leftLowerArm = new Sprite("left_lower_arm.png");
        leftUpperArm.AddChild(leftLowerArm);
        leftLowerArm.SetOrigin(leftLowerArm.width - leftLowerArm.height / 2, leftLowerArm.height / 2);
        leftLowerArm.SetXY(-leftUpperArm.width + leftLowerArm.height / 2, 0);

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