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

    public Interactable(string filename, int cols, int rows, TiledObject obj = null) : base(filename, cols, rows) {
        done = false;
        this.type = obj.GetStringProperty("type");
        size = 16;
        SetOrigin(width / 2, height / 2);
        myGame = (MyGame)game;
    }

    void Update() {
        if (done)
            alpha = 0;

        if (myGame.player != null && !added) {
            myGame.player.interactables.Add(this);
            added = true;
        }
    }
}