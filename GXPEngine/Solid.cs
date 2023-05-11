using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class Solid : Sprite {
    Line line;
    MyGame myGame;

    public Solid() : base("solid.png") {
        myGame = (MyGame)game;

        SetOrigin(width / 2, height / 2);

        line = new Line(-width / 2, -height / 2, width / 2, -height / 2);
        AddChild(line);
        myGame.lines.Add(line);
        line = new Line(width / 2, -height / 2, width / 2, height / 2);
        AddChild(line);
        myGame.lines.Add(line);
        line = new Line(width / 2, height / 2, -width / 2, height / 2);
        AddChild(line);
        myGame.lines.Add(line);
        line = new Line(-width / 2, height / 2, -width / 2, -height / 2);
        AddChild(line);
        myGame.lines.Add(line);
    }
}