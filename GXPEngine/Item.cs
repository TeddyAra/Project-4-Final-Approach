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

    public Vec2 pos;
    public Vec2 vel;
    int radius;

    bool check;

    public Item(string filename, int cols, int rows, TiledObject obj = null) : base(filename, cols, rows) {
        myGame = (MyGame)game;

        SetOrigin(width / 2, height / 2);

        this.type = obj.GetStringProperty("type");
        this.pos = new Vec2(obj.X, obj.Y);
        radius = (int)obj.Width;

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
    }

    void Update() {
        SetXY(pos.x, pos.y);
        pos += vel;

        if (!check) {
            if (myGame.lines != null) {
                this.lines = myGame.lines;
                check = true;
            }
        } else {
            for (int i = lines.Count() - 1; i > -1; i--) { 
                if (lines[i].item == null) {
                    float projection = (lines[i].point2 - lines[i].point1).Normalized().Dot(pos - new Vec2(lines[i].TransformPoint(0, 0).x + lines[i].point1.x, lines[i].TransformPoint(0, 0).y + lines[i].point1.y));
                    Vec2 impactPoint = new Vec2(lines[i].TransformPoint(0, 0).x + lines[i].point1.x, lines[i].TransformPoint(0, 0).y + lines[i].point1.y) + (lines[i].point2 - lines[i].point1).Normalized() * projection;

                    if (projection > 0 && projection < (lines[i].point1 - lines[i].point2).Length() && (impactPoint - pos).Length() < radius) {
                        vel.Reflect((lines[i].point2 - lines[i].point1).Normal(), 0.95f);

                        if (lines[i].door != null && lines[i].door.type == "goal" && type == "areaBall" && myGame.player.pos.x < 900) {
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