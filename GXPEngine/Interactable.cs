using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using TiledMapParser;

public class Interactable : AnimationSprite {
    public String type;
    public bool done;
    public int size;
    bool added;
    MyGame myGame;
    List<Line> lines;
    int radius;

    Vec2 pos;
    Vec2 vel;
    int bounced;
    bool check;
    Sound bounce;

    public Interactable(string filename, int cols, int rows, TiledObject obj = null) : base(filename, cols, rows) {
        done = false;
        this.type = obj.GetStringProperty("type");
        size = 16;
        SetOrigin(width / 2, height / 2);
        myGame = (MyGame)game;
        bounce = new Sound("object hitting the surface.wav", false, true);
        radius = (int)obj.Width / 2;

        this.pos = new Vec2(obj.X, obj.Y);
        if (type == "map1" || type == "map2" || type == "map3")
            vel = Vec2.RandomUnitVector();
    }

    void Update() {
        if (done)
            alpha = 0;

        if (myGame.player != null && !added) {
            myGame.player.interactables.Add(this);
            added = true;
        }

        SetXY(pos.x, pos.y);
        pos += vel;
        bounced--;
        
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
                        pos -= vel;
                        vel.Reflect((lines[i].point2 - lines[i].point1).Normal(), 0.95f);
                        if (bounced < 0)
                            bounce.Play();
                        bounced = 60;
                    }
                }
            }
        }
    }
}