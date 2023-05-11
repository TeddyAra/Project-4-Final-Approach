using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using GXPEngine;

public class Item : Sprite {
    Line line;
    MyGame myGame;
    String type;
    List<Line> lines;

    public Vec2 pos;
    public Vec2 vel;
    int radius;

    public Item(String type, List<Line> lines) : base("solid.png") {
        myGame = (MyGame)game;

        SetOrigin(width / 2, height / 2);

        this.type = type;
        this.lines = lines;
        radius = width / 2;

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

        if (type == "areaBall") {
            SetColor(255, 0, 0);
            if (pos.x > game.width / 3 * 2) {
                Console.WriteLine("uwu :3");
            }
        }

        for (int i = 0; i < lines.Count; i++) { 
            if (lines[i].item == null) {
                float projection = (lines[i].point2 - lines[i].point1).Normalized().Dot(pos - new Vec2(lines[i].TransformPoint(0, 0).x + lines[i].point1.x, lines[i].TransformPoint(0, 0).y + lines[i].point1.y));
                Vec2 impactPoint = new Vec2(lines[i].TransformPoint(0, 0).x + lines[i].point1.x, lines[i].TransformPoint(0, 0).y + lines[i].point1.y) + (lines[i].point2 - lines[i].point1).Normalized() * projection;

                if (projection > 0 && projection < (lines[i].point1 - lines[i].point2).Length() && (impactPoint - pos).Length() < radius) {
                    vel.Reflect((lines[i].point2 - lines[i].point1).Normal());
                }
            }
        }
    }
}