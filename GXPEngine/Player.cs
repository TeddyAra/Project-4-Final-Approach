using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using GXPEngine;
using GXPEngine.Core;
using SharpDX.DirectInput;
using SharpDX.XInput;
using TiledMapParser;

public class Player : EasyDraw {
    AnimationSprite body;
    AnimationSprite tongue;
    AnimationSprite tongueTipImage;
    List<Sprite> tongues;
    EasyDraw head1;
    AnimationSprite head2;

    MyGame myGame;
    List<Line> lines;
    public List<Item> items;
    public List<Interactable> interactables;
    bool againstSolid;
    bool holdingItem;
    Vec2 tongueTip;
    Vec2 oldTongueTip;
    Vec2 tongueDirectionVec;
    Item heldItem;

    int bodyHeightHalf; // Y position of the origin point of the body
    int tongueNumber; // How many pieces the tongue should be made out of

    public Vec2 pos; // The body's position
    Vec2 vel; // The body's movement velocity
    Vec2 force; // The force applied on the body
    int mass; // The body's mass
    float rotVel; // The body's rotation velocity
    int headAngle; // The angle of the head and the tongue
    int headRotSpeed; // The speed that the head rotates with
    int headMaxAngle; // The max angle the head can differ from looking straight
    float tongueLength; // The length of the tongue that's sticking out
    int tongueSpeed; // How fast the tongue moves
    int grabCooldown;
    float bodyRotation;
    float headRotation;

    bool checkingMap;
    Sprite map;
    Sprite mapNotif;
    List<int> collectedMaps;

    bool codeInput;
    bool codeFinished;
    Sprite codeDone;
    Sprite codeNotDone;
    Sprite select;
    List<int> input;
    String codeInputString;
    List<int> solution;
    EasyDraw code;
    Font newFont;
    int selection;
    bool buttonReleased;
    bool buttonPressed;
    bool moving;
    bool buttonReset;

    Sprite pause;
    Sprite controls;
    Sprite menuSelect;
    bool paused;
    bool controlsEnabled;
    int menuSelection;

    Sprite ui1;
    Sprite ui2;
    Sprite ui3;
    Sprite ui4;
    Sprite barBackground;
    public Sprite bar;
    float percentage;

    Joystick joystick;
    JoystickState state;

    Sound collect;
    Sound codeCorrect;
    Sound door;
    Sound incorrect;
    Sound button;
    Sound hit;
    Sound fold;
    Sound tongueSound;
    int tongueSoundPlaying;

    public Player(Guid joystickGuid, List<Line> lines, List<Interactable> interactables) : base(2, 2) {
        myGame = (MyGame)game;
        this.lines = lines;
        this.interactables = interactables;
        items = new List<Item>();
        collectedMaps = new List<int>();

        newFont = new Font("good times rg.otf", 40);

        headMaxAngle = 45;
        tongueNumber = 7;
        tongueLength = 300;

        pos = new Vec2(25 * 64, 6 * 64);
        tongueSpeed = 3;

        SetupBody();

        joystick = new Joystick(new DirectInput(), joystickGuid);
        joystick.Properties.BufferSize = 32;
        joystick.Properties.DeadZone = 500;

        map = new Sprite("mapBackground.png");
        game.LateAddChild(map);
        map.SetOrigin(map.width / 2, map.height / 2);
        map.alpha = 0;
        mapNotif = new Sprite("mapNotif.png");
        game.LateAddChild(mapNotif);
        mapNotif.SetOrigin(mapNotif.width / 2, mapNotif.height / 2);
        mapNotif.alpha = 0;

        codeDone = new Sprite("codeYes.png");
        game.LateAddChild(codeDone);
        codeDone.SetOrigin(codeDone.width / 2, codeDone.height / 2);
        codeNotDone = new Sprite("codeNo.png");
        game.LateAddChild(codeNotDone);
        codeNotDone.SetOrigin(codeNotDone.width / 2, codeNotDone.height / 2);
        select = new Sprite("select.png");
        game.LateAddChild(select);
        select.SetOrigin(select.width / 2, select.height / 2);
        codeDone.alpha = 0;
        codeNotDone.alpha = 0;
        select.alpha = 0;
        code = new EasyDraw(1920, 1080);
        game.LateAddChild(code);
        code.SetOrigin(code.width / 2, code.height / 2);

        ui1 = new Sprite("map1ui.png");
        game.LateAddChild(ui1);
        ui1.SetOrigin(game.width / 2, game.height / 2);
        ui1.alpha = 0;
        ui2 = new Sprite("map2ui.png");
        game.LateAddChild(ui2);
        ui2.SetOrigin(game.width / 2, game.height / 2);
        ui2.alpha = 0;
        ui3 = new Sprite("map3ui.png");
        game.LateAddChild(ui3);
        ui3.SetOrigin(game.width / 2, game.height / 2);
        ui3.alpha = 0;
        ui4 = new Sprite("map4ui.png");
        game.LateAddChild(ui4);
        ui4.SetOrigin(game.width / 2, game.height / 2);
        ui4.alpha = 0;
        barBackground = new Sprite("barBackground.png");
        game.LateAddChild(barBackground);
        barBackground.SetOrigin(barBackground.width / 2, barBackground.height / 2);
        bar = new Sprite("bar.png");
        game.LateAddChild(bar);
        bar.width = 0;

        pause = new Sprite("pause.png");
        game.LateAddChild(pause);
        pause.SetOrigin(pause.width / 2, pause.height / 2);
        controls = new Sprite("controls.png");
        game.LateAddChild(controls);
        controls.SetOrigin(controls.width / 2, controls.height / 2);
        menuSelect = new Sprite("menuSelect.png");
        game.LateAddChild(menuSelect);
        menuSelect.SetOrigin(menuSelect.width / 2, menuSelect.height / 2);
        menuSelection = 1;
        pause.alpha = 0;
        controls.alpha = 0;
        menuSelect.alpha = 0;

        input = new List<int>();
        solution = new List<int> { 
            1, 5, 3, 9    
        };

        collect = new Sound("collecting paper sound.wav", false, true);
        codeCorrect = new Sound("correct code 3.wav", false, true);
        incorrect = new Sound("incorrect code sound.wav", false, true);
        button = new Sound("inserting a number possible.wav", false, true);
        hit = new Sound("object hitting the surface.wav", false, true);
        fold = new Sound("paper folding sound.wav", false, true);
        tongueSound = new Sound("tongue_sticking_out_louder.wav", false, true);
    }

    void Update() {
        joystick.Acquire();
        state = joystick.GetCurrentState();

        if (paused)
            Menu();
        else if (checkingMap)
            MapUI();
        else if (codeInput)
            CodeUI();
        else
            Play();

        if (collectedMaps.Count() == 0) {
            ui1.alpha = 1;
        } else if (collectedMaps.Count() == 1) {
            ui1.alpha = 0;
            ui2.alpha = 1;
        } else if (collectedMaps.Count() == 2) {
            ui2.alpha = 0;
            ui3.alpha = 1;
        } else if (collectedMaps.Count() == 3) {
            ui3.alpha = 0;
            ui4.alpha = 1;
        }

        ui1.SetXY(pos.x, pos.y);
        ui2.SetXY(pos.x, pos.y);
        ui3.SetXY(pos.x, pos.y);
        ui4.SetXY(pos.x, pos.y);

        barBackground.SetXY(pos.x, pos.y);
        bar.SetXY(pos.x - 349, pos.y - game.height / 2 + 55);

        myGame.camera.rotation = -rotation;
    }

    void Menu() {
        if (!state.Buttons[1] && !state.Buttons[2] && !state.Buttons[9]) {
            buttonPressed = false;
        }
            
        if (!controlsEnabled) {
            pause.alpha = 1;
            controls.alpha = 0;
            menuSelect.alpha = 1;
            pause.SetXY(pos.x, pos.y);
            controls.SetXY(pos.x, pos.y);

            if ((state.Buttons[9] || state.Buttons[2]) && !buttonPressed) {
                paused = false;
                buttonPressed = true;
                pause.alpha = 0;
                menuSelect.alpha = 0;
            }

            if (state.Buttons[1] && !buttonPressed) {
                button.Play();
                if (menuSelection == 1) {
                    paused = false;
                    buttonPressed = true;
                    pause.alpha = 0;
                    menuSelect.alpha = 0;
                } else if (menuSelection == 2) {
                    controlsEnabled = true;
                    buttonPressed = true;
                } else if (menuSelection == 3) {
                    // Settings
                } else if (menuSelection == 4) {
                    // Main menu
                }
            }

            int arrowValue = state.PointOfViewControllers[0];
            if (buttonReleased) {
                if (arrowValue != -1) {
                    button.Play();
                    buttonReleased = false;
                }

                if (arrowValue == 0) {
                    if (menuSelection > 1)
                        menuSelection--;
                } else if (arrowValue == 18000) {
                    if (menuSelection < 4)
                        menuSelection++;
                }
            }
            if (arrowValue == -1)
                buttonReleased = true;

            menuSelect.SetXY(pos.x, pos.y - 120 + (menuSelection - 1) * 140);
        } else {
            controls.alpha = 1;
            pause.alpha = 0;
            menuSelect.alpha = 0;

            if (state.Buttons[9] && !buttonPressed) {
                paused = false;
                buttonPressed = true;
                controls.alpha = 0;
            }

            if (state.Buttons[2] && !buttonPressed) {
                controlsEnabled = false;
                buttonPressed = true;
            }
        }
    }

    void CodeUI() {
        codeDone.SetXY(pos.x, pos.y);
        codeNotDone.SetXY(pos.x, pos.y);

        if (codeFinished) {
            codeDone.alpha = 1;
            codeNotDone.alpha = 0;
            select.alpha = 0;
        } else {
            codeNotDone.alpha = 1;
            select.alpha = 1;

            if (codeFinished) {
                codeDone.alpha = 1;
            } else {
                codeNotDone.alpha = 1;
            }

            for (int i = 0; i < input.Count(); i++) {
                codeInputString = codeInputString + input[i];
            }

            code.ClearTransparent();
            code.TextSize(40);
            code.SetXY(pos.x, pos.y + 270);
            code.TextFont(newFont);
            code.Fill(53, 57, 54);
            code.Text(codeInputString);
            code.TextAlign(CenterMode.Center, CenterMode.Min);

            int arrowValue = state.PointOfViewControllers[0];
            if (buttonReleased) {
                if (arrowValue != -1) {
                    buttonReleased = false;
                }

                if (arrowValue == 0) {
                    if (selection > 3) {
                        selection -= 3;
                    }
                } else if (arrowValue == 9000) {
                    if (selection < 12) {
                        selection++;
                    }
                } else if (arrowValue == 18000) {
                    if (selection < 10) {
                        selection += 3;
                    }
                } else if (arrowValue == 27000) {
                    if (selection > 1) {
                        selection--;
                    }
                }
            }
            if (arrowValue == -1)
                buttonReleased = true;

            if (state.Buttons[1] && !buttonPressed) {
                if (input.Count() < 7) {
                    if (selection < 10) {
                        input.Add(selection);
                        button.Play();
                    }
                    if (selection == 11) {
                        input.Add(0);
                    }
                }
                if (selection == 10) {
                    if (input.Count() == 4) {
                        bool correct = true;
                        for (int i = 0; i < input.Count(); i++) {
                            if (input[i] != solution[i]) {
                                correct = false;
                            }
                        }

                        if (correct) {
                            codeCorrect.Play();
                            codeFinished = true;
                            for (int i = myGame.doors.Count() - 1; i > -1; i--) {
                                if (myGame.doors[i].type == "door") {
                                    myGame.doors[i].Destroy();
                                    bar.width = 560;
                                }
                            }
                        } else {
                            input.Clear();
                            incorrect.Play();
                        }
                    } else {
                        input.Clear();
                    }
                }
                if (selection == 12) {
                    input.Clear();
                }

                buttonPressed = true;
            }

            if (!state.Buttons[1]) {
                buttonPressed = false;
            }
            

            if (selection == 1 || selection == 2 || selection == 3) {
                select.y = pos.y - 150;
            } else if (selection == 7 || selection == 8 || selection == 9) {
                select.y = pos.y + 150;
            } else if (selection == 10 || selection == 11 || selection == 12) {
                select.y = pos.y + 300;
            } else {
                select.y = pos.y;
            }

            if (selection == 1 || selection == 4 || selection == 7 || selection == 10) {
                select.x = pos.x - 150;
            } else if (selection == 3 || selection == 6 || selection == 9 || selection == 12) {
                select.x = pos.x + 150;
            } else {
                select.x = pos.x;
            }

            codeInputString = "";
        }

        if (state.Buttons[2]) {
            codeInput = false;
            codeNotDone.alpha = 0;
            codeDone.alpha = 0;
            select.alpha = 0;
            code.alpha = 0;
        }
    }

    void MapUI() {
        map.SetXY(pos.x, pos.y);
        map.alpha = 1;

        if (state.Buttons[2]) {
            map.alpha = 0;
            checkingMap = false;
            fold.Play();
        }
    }

    void Play() {
        moving = false;
        pos += vel;
        SetXY(pos.x, pos.y);
        grabCooldown--;
        mapNotif.SetXY(pos.x, pos.y);

        // Updating tonguetip
        tongueDirectionVec = new Vec2(tongues[tongues.Count() - 1].TransformDirection(1, 0).x, tongues[tongues.Count() - 1].TransformDirection(1, 0).y).RotatedDeg(-90);
        oldTongueTip = tongueTip;
        tongueTip = new Vec2(tongues[tongues.Count() - 1].TransformPoint(0, 0).x, tongues[tongues.Count() - 1].TransformPoint(0, 0).y) + tongueDirectionVec * tongues[0].height;

        // Moving body and head
        headRotation += (state.Z - (65535 / 2)) / 30000;
        head1.rotation = headRotation;
        if (CheckCollision() || headRotation < -60 || headRotation > 60) {
            headRotation -= (state.Z - (65535 / 2)) / 30000;
            head1.rotation = headRotation;
        }

        float oldBodyRotation = bodyRotation;
        bodyRotation += (state.X - (65535 / 2)) / 30000;
        rotation = bodyRotation;
        if (againstSolid) {
            pos.RotateAroundDeg((state.X - (65535 / 2)) / 30000, tongueTip);
            if (Mathf.Abs(oldBodyRotation - bodyRotation) != 0)
                moving = true;
            if (BodyCollision()) {
                bodyRotation -= (state.X - (65535 / 2)) / 30000;
                rotation = bodyRotation;
                pos.RotateAroundDeg(-(state.X - (65535 / 2)) / 30000, tongueTip);
                moving = false;
            }
        } else if (CheckCollision()) {
            bodyRotation -= (state.X - (65535 / 2)) / 30000;
            rotation = bodyRotation;
        }

        // Grabbing mechanic
        if (state.Buttons[5] && !buttonReset) {
            buttonReset = true;
            if (holdingItem || againstSolid) {
                if (holdingItem) {
                    heldItem.vel = tongueTip - oldTongueTip;
                    holdingItem = false;
                    grabCooldown = 60;
                } else {
                    againstSolid = false;
                }
            } else {
                for (int i = 0; i < interactables.Count(); i++) {
                    if ((new Vec2(interactables[i].x, interactables[i].y) - tongueTip).Length() < interactables[i].size) {
                        if (!interactables[i].done) {
                            if (interactables[i].type == "map1") {
                                collectedMaps.Add(1);
                                interactables[i].done = true;
                                if (bar.width < 500) 
                                    bar.width += 140;
                                collect.Play();
                            } else if (interactables[i].type == "map2") {
                                collectedMaps.Add(2);
                                interactables[i].done = true;
                                if (bar.width < 500)
                                    bar.width += 140;
                                collect.Play();
                            } else if (interactables[i].type == "map3") {
                                collectedMaps.Add(3);
                                interactables[i].done = true;
                                if (bar.width < 500)
                                    bar.width += 140;
                                collect.Play();
                            } else if (interactables[i].type == "code") {
                                codeInput = true;
                                buttonPressed = true;
                                selection = 5;
                                code.alpha = 1;
                            }
                            goto skip;
                        }
                    }
                }

                for (int i = 0; i < lines.Count(); i++) {
                    float projection = (lines[i].point2 - lines[i].point1).Normalized().Dot(tongueTip - new Vec2(lines[i].TransformPoint(0, 0).x + lines[i].point1.x, lines[i].TransformPoint(0, 0).y + lines[i].point1.y));
                    Vec2 impactPoint = new Vec2(lines[i].TransformPoint(0, 0).x + lines[i].point1.x, lines[i].TransformPoint(0, 0).y + lines[i].point1.y) + (lines[i].point2 - lines[i].point1).Normalized() * projection;

                    if (projection > 0 && projection < (lines[i].point1 - lines[i].point2).Length() && (impactPoint - tongueTip).Length() < 20) {
                        for (int j = 0; j < items.Count(); j++) {
                            if ((tongueTip - new Vec2(items[j].x, items[j].y)).Length() < 20) {
                                holdingItem = true;
                                heldItem = items[j];
                                goto skip;
                            }
                            againstSolid = true;
                        }
                    }
                }
            }
        }

    skip:

        if (!state.Buttons[5]) {
            buttonReset = false;
        }

        if (state.Buttons[6]) {
            if (tongueSoundPlaying < 0) {
                tongueSound.Play();
                tongueSoundPlaying = 80;
            }
            if (againstSolid && tongues[0].y < (tongues.Count() - 1) * tongues[0].height - 15) { 
                pos += Vec2.GetUnitVectorDeg(head1.rotation - 90 + rotation) * tongueSpeed;
                tongueLength += tongueSpeed;
                moving = true;
            } else if (!againstSolid && tongues[0].y < (tongues.Count() - 1) * tongues[0].height) {
                tongueLength += tongueSpeed;
                if (CheckCollision())
                    tongueLength += tongueSpeed * 1.5f;
            }
        }

        if (state.Buttons[7] && tongues[0].y > 0) {
            if (tongueSoundPlaying < 0) {
                tongueSound.Play();
                tongueSoundPlaying = 80;
            }
            tongueLength -= tongueSpeed;
            if (againstSolid) {
                pos -= Vec2.GetUnitVectorDeg(head1.rotation - 90 + rotation) * tongueSpeed;
                moving = true;
            } else if (CheckCollision())
                tongueLength += tongueSpeed * 1.5f;
        }

        tongueSoundPlaying--;

        if (holdingItem) {
            heldItem.pos = new Vec2(tongueTip.x, tongueTip.y);
        }

        tongues[0].SetXY(0, tongueLength);
        for (int i = 0; i < tongues.Count(); i++) {
            if (tongues[0].y - (i + 0.5f) * tongues[0].height > 0) {
                tongues[i].alpha = 0;
            } else {
                tongues[i].alpha = 1;
            }
        }

        if (state.Buttons[3]) {
            if (collectedMaps.Count() == 3) {
                checkingMap = true;
                fold.Play();
            }
        }

        if (collectedMaps.Count() == 3)
            mapNotif.alpha = 1;

        if (moving) {
            body.SetCycle(0, 14);
        } else {
            body.SetCycle(14, 6);
        }

        if (state.Buttons[9] && !buttonPressed) {
            paused = true;
            controlsEnabled = false;
            buttonPressed = true;
        }

        if (!state.Buttons[9]) {
            buttonPressed = false;
        }

        body.Animate(0.15f);
        head2.Animate(0.1f);

        if (againstSolid) {
            tongueTipImage.SetCycle(4);
        } else {
            tongueTipImage.SetCycle(3);
        }
    }

    bool CheckCollision() { 
        tongueDirectionVec = new Vec2(tongues[tongues.Count() - 1].TransformDirection(1, 0).x, tongues[tongues.Count() - 1].TransformDirection(1, 0).y).RotatedDeg(-90);
        tongueTip = new Vec2(tongues[tongues.Count() - 1].TransformPoint(0, 0).x, tongues[tongues.Count() - 1].TransformPoint(0, 0).y) + tongueDirectionVec * tongues[0].height;

        for (int i = 0; i < lines.Count(); i++) {
            float projection = (lines[i].point2 - lines[i].point1).Normalized().Dot(tongueTip - new Vec2(lines[i].TransformPoint(0, 0).x + lines[i].point1.x, lines[i].TransformPoint(0, 0).y + lines[i].point1.y));
            Vec2 impactPoint = new Vec2(lines[i].TransformPoint(0, 0).x + lines[i].point1.x, lines[i].TransformPoint(0, 0).y + lines[i].point1.y) + (lines[i].point2 - lines[i].point1).Normalized() * projection;

            if (projection > 0 && projection < (lines[i].point1 - lines[i].point2).Length() && (impactPoint - tongueTip).Length() < 3) {
                if (lines[i].item == null) {
                    return true;
                }
            }
        }

        return false;
    }

    bool BodyCollision() { 
        for (int i = 0; i < lines.Count(); i++) {
            float projection = (lines[i].point2 - lines[i].point1).Normalized().Dot(pos - new Vec2(lines[i].TransformPoint(0, 0).x + lines[i].point1.x, lines[i].TransformPoint(0, 0).y + lines[i].point1.y));
            Vec2 impactPoint = new Vec2(lines[i].TransformPoint(0, 0).x + lines[i].point1.x, lines[i].TransformPoint(0, 0).y + lines[i].point1.y) + (lines[i].point2 - lines[i].point1).Normalized() * projection;

            if (projection > 0 && projection < (lines[i].point1 - lines[i].point2).Length() && (impactPoint - pos).Length() < 3) {
                if (lines[i].item == null) {
                    return true;
                }
            }
        }

        return false;
    }

    void SetupBody() { 
        bodyHeightHalf = 25;

        SetOrigin(width / 2, height / 2);

        body = new AnimationSprite("body.png", 5, 4);
        AddChild(body);
        body.SetOrigin(body.width / 2, bodyHeightHalf);
        body.SetXY(width / 2, height / 2);

        head1 = new EasyDraw(2, 2);
        body.AddChild(head1);
        head1.SetOrigin(head1.width / 2, head1.height / 2);
        head1.SetXY(0, -bodyHeightHalf);

        tongues = new List<Sprite>();
        tongue = new AnimationSprite("tongue.png", 5, 1);
        head1.AddChild(tongue);
        tongue.SetOrigin(tongue.width / 2, tongue.height);
        tongue.SetXY(0, -bodyHeightHalf);
        tongue.SetFrame(0);
        tongues.Add(tongue);

        for (int i = 0; i < tongueNumber - 2; i++) {
            tongue = new AnimationSprite("tongue.png", 5, 1);
            tongues[tongues.Count() - 1].AddChild(tongue);
            tongue.SetOrigin(tongue.width / 2, tongue.height);
            tongue.SetXY(0, -tongue.height);
            tongue.SetFrame(1);
            tongues.Add(tongue);
        }

        tongue = new AnimationSprite("tongue.png", 5, 1);
        tongues[tongues.Count() - 1].AddChild(tongue);
        tongue.SetOrigin(tongue.width / 2, tongue.height);
        tongue.SetXY(0, -tongue.height);
        tongue.SetFrame(2);
        tongues.Add(tongue);

        tongueTipImage = new AnimationSprite("tongue.png", 5, 1);
        tongues[tongues.Count() - 1].AddChild(tongueTipImage);
        tongueTipImage.SetOrigin(tongueTipImage.width / 2, tongueTipImage.height);
        tongueTipImage.SetXY(0, -tongueTipImage.height);
        tongueTipImage.SetFrame(3);
        tongues.Add(tongueTipImage);

        head2 = new AnimationSprite("head.png", 5, 4);
        head1.AddChild(head2);
        head2.SetOrigin(head2.width / 2, head2.height / 2);
    }
}