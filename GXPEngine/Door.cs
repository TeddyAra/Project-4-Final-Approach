using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using TiledMapParser;

public class Door : AnimationSprite {
    Line line;
    MyGame myGame;
    bool check;

    public String type;

    public Door(string filename, int cols, int rows, TiledObject obj = null) : base(filename, cols, rows) {
        myGame = (MyGame)game;
        this.type = obj.GetStringProperty("type");

        SetOrigin(width / 2, height / 2);
    }

    void Update() {
        if (!check) {
            myGame.doors.Add(this);
            check = true;

            if (currentFrame == 87 || currentFrame == 114 || currentFrame == 141) {
                line = new Line(width / 4, -height / 2, width / 2, -height / 2, null, null, this);
                AddChild(line);
                myGame.lines.Add(line);
                line = new Line(width / 2, -height / 2, width / 2, height / 2, null, null, this);
                AddChild(line);
                myGame.lines.Add(line);
                line = new Line(width / 2, height / 2, width / 4, height / 2, null, null, this);
                AddChild(line);
                myGame.lines.Add(line);
                line = new Line(width / 4, height / 2, width / 4, -height / 2, null, null, this);
                AddChild(line);
                myGame.lines.Add(line);
            } else {
                line = new Line(-width / 2, -height / 2, -width / 4, -height / 2, null, null, this);
                AddChild(line);
                myGame.lines.Add(line);
                line = new Line(-width / 4, -height / 2, -width / 4, height / 2, null, null, this);
                AddChild(line);
                myGame.lines.Add(line);
                line = new Line(-width / 4, height / 2, -width / 2, height / 2, null, null, this);
                AddChild(line);
                myGame.lines.Add(line);
                line = new Line(-width / 2, height / 2, -width / 2, -height / 2, null, null, this);
                AddChild(line);
                myGame.lines.Add(line);
            }
        }
    }
}