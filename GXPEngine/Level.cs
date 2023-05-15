using System;
using GXPEngine;
using TiledMapParser;


namespace GXPEngine {
    public class Level : GameObject {

        TiledLoader loader;
        MyGame myGame;
        bool check;

        public Level(string filename) {
            loader = new TiledLoader(filename);
            CreateLevel();

            myGame = (MyGame)game;
        }

        void Update() {
            if (!check) { 
                Sprite[] sprites = FindObjectsOfType<Sprite>();
                foreach (Sprite sprite in sprites) {
                    if (sprite.name == "space") {
                        myGame.space = sprite;
                        check = true;
                    }
                }
            }
        }

        void CreateLevel(bool IncludeImageLayer = true) {
            loader.rootObject = this;

            loader.addColliders = false;
            loader.LoadImageLayers();

            loader.LoadTileLayers();
            loader.addColliders = true;
            loader.autoInstance = true;
            loader.LoadObjectGroups();
        }
    }
}