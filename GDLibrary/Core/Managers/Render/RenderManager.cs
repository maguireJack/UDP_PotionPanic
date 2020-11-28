using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.GameComponents;
using Microsoft.Xna.Framework;

namespace GDLibrary.Managers
{
    /// <summary>
    /// Renders all the objects in the ObjectManager lists
    /// </summary>
    public class RenderManager : PausableDrawableGameComponent
    {
        private ScreenLayoutType screenLayoutType;
        private ObjectManager objectManager;
        private CameraManager<Camera3D> cameraManager;

        public RenderManager(Game game, StatusType statusType,
            ScreenLayoutType screenLayoutType,
            ObjectManager objectManager,
            CameraManager<Camera3D> cameraManager) : base(game, statusType)
        {
            this.screenLayoutType = screenLayoutType;
            this.objectManager = objectManager;
            this.cameraManager = cameraManager;
        }

        /// <summary>
        /// Called to draw the lists of actors
        /// </summary>
        /// <see cref="PausableDrawableGameComponent.Draw(GameTime)"/>
        /// <param name="gameTime">GameTime object</param>
        protected override void ApplyDraw(GameTime gameTime)
        {
            if (this.screenLayoutType == ScreenLayoutType.Single)
                DrawSingle(gameTime, cameraManager.ActiveCamera);
            else
                DrawMulti(gameTime);

            // base.ApplyDraw(gameTime);
        }

        private void DrawMulti(GameTime gameTime)
        {
            for (int i = 0; i < 4; i++)
            {
                DrawSingle(gameTime, cameraManager[i]);
            }
        }

        private void DrawSingle(GameTime gameTime, Camera3D activeCamera)
        {
            this.GraphicsDevice.Viewport = activeCamera.Viewport;

            foreach (DrawnActor3D actor in objectManager.OpaqueList)
            {
                if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
                    actor.Draw(gameTime, activeCamera, GraphicsDevice);
            }

            foreach (DrawnActor3D actor in objectManager.TransparentList)
            {
                if ((actor.StatusType & StatusType.Drawn) == StatusType.Drawn)
                    actor.Draw(gameTime, activeCamera, GraphicsDevice);
            }
        }
    }
}