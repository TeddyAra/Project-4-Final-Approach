using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using TiledMapParser;

public class Solid : AnimationSprite {
    Line line;
    MyGame myGame;

    public String type;

    public Solid(TiledObject obj = null) : base("solid.png", 1, 1) {
        myGame = (MyGame)game;
        this.type = obj.GetStringProperty("type");
        alpha = 0;

        this.width = (int)obj.Width;
        this.height = (int)obj.Height;

        SetOrigin(width / 2, height / 2);

        line = new Line(-width / 2, -height / 2, width / 2, -height / 2, null, this);
        AddChild(line);
        myGame.lines.Add(line);
        line = new Line(width / 2, -height / 2, width / 2, height / 2, null, this);
        AddChild(line);
        myGame.lines.Add(line);
        line = new Line(width / 2, height / 2, -width / 2, height / 2, null, this);
        AddChild(line);
        myGame.lines.Add(line);
        line = new Line(-width / 2, height / 2, -width / 2, -height / 2, null, this);
        AddChild(line);
        myGame.lines.Add(line);

        if (type == "goal") {
            SetColor(0, 0, 1);
        }
    }
}