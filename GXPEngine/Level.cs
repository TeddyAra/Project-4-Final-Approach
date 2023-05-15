using GXPEngine;
using TiledMapParser;


namespace GXPEngine {
    public class Level : GameObject {

        TiledLoader loader;

        public Level(string filename) {
            loader = new TiledLoader(filename);
            CreateLevel();
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