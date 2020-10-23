using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary
{
    internal class DebugDrawer : IGameComponent
    {
        private Main main;
        private SpriteBatch _spriteBatch;
        private SpriteFont debugFont;
        private CameraManager cameraManager;
        private ObjectManager objectManager;

        public DebugDrawer(Main main, SpriteBatch spriteBatch, SpriteFont debugFont, CameraManager cameraManager, ObjectManager objectManager)
        {
            this.main = main;
            _spriteBatch = spriteBatch;
            this.debugFont = debugFont;
            this.cameraManager = cameraManager;
            this.objectManager = objectManager;
        }

        public void Initialize()
        {
            throw new System.NotImplementedException();
        }

        public void Draw()
        {

        }
    }
}