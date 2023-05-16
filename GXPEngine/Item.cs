using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using GXPEngine;
using TiledMapParser;

public class Item : AnimationSprite {
    Line line;
    MyGame myGame;
    String type;
    List<Line> lines;
    int bounced;

    public Vec2 pos;
    public Vec2 vel;
    int radius;
    bool closing;
    int closeTimer;

    bool check;
    Sound door;
    Sound bounce;

    public Item(string filename, int cols, int rows, TiledObject obj = null) : base(filename, cols, rows) {
        myGame = (MyGame)game;
        closeTimer = 120;

        SetOrigin(width / 2, height / 2);

        this.type = obj.GetStringProperty("type");
        this.pos = new Vec2(obj.X, obj.Y);
        radius = (int)obj.Width / 2;

        line = new Line(-width / 2, -height / 2, width / 2, -height / 2, this);
        AddChild(line);
        myGame.lines.Add(line);
        line = new Line(width / 2, -height / 2, width / 2, height / 2, this);
        AddChild(line);
        myGame.lines.Add(line);
        line = new Line(width / 2, height / 2, -width / 2, height / 2, this);
        AddChild(line);
        myGame.lines.Add(line);
        line = new Line(-width / 2, height / 2, -width / 2, -height / 2, this);
        AddChild(line);
        myGame.lines.Add(line);

        door = new Sound("door smashing sound.wav", false, true);
        bounce = new Sound("object hitting the surface.wav", false, true);

        vel = Vec2.RandomUnitVector();
    }

    void Update() {
        SetXY(pos.x, pos.y);
        pos += vel;
        bounced--;

        if (closing) {
            closeTimer--;
            if (closeTimer < 0)
                Environment.Exit(1);
        }
        
        if (!check) {
            if (myGame.player != null) {
                this.lines = myGame.lines;
                myGame.player.items.Add(this);
                check = true;
            }
        } else {
            for (int i = lines.Count() - 1; i > -1; i--) { 
                if (lines[i].item == null) {
                    float projection = (lines[i].point2 - lines[i].point1).Normalized().Dot(pos - new Vec2(lines[i].TransformPoint(0, 0).x + lines[i].point1.x, lines[i].TransformPoint(0, 0).y + lines[i].point1.y));
                    Vec2 impactPoint = new Vec2(lines[i].TransformPoint(0, 0).x + lines[i].point1.x, lines[i].TransformPoint(0, 0).y + lines[i].point1.y) + (lines[i].point2 - lines[i].point1).Normalized() * projection;

                    if (projection > 0 && projection < (lines[i].point1 - lines[i].point2).Length() && (impactPoint - pos).Length() < radius) {
                        pos -= vel;
                        vel.Reflect((lines[i].point2 - lines[i].point1).Normal(), 0.95f);
                        if (bounced < 0)
                            bounce.Play();
                        bounced = 60;

                        if (lines[i].door != null && lines[i].door.type == "goal" && type == "areaBall") {
                            closing = true;
                            myGame.player.bar.width = 700;
                            door.Play();
                            for (int j = myGame.doors.Count() - 1; j > -1; j--) {
                                myGame.doors[j].LateDestroy();
                            }
                        } 
                    }
                }
            }
        }
    }
}