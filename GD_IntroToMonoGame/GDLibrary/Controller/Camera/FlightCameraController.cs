using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace GDLibrary
{

    //to do - generalise move keys, remove (512, 384)
    public class FlightCameraController : IController
    {
        private KeyboardManager keyboardManager;
        private MouseManager mouseManager;
        private float moveSpeed, strafeSpeed, rotationSpeed;

        public FlightCameraController(KeyboardManager keyboardManager,
            MouseManager mouseManager,
            float moveSpeed,
            float strafeSpeed, float rotationSpeed)
        {
            this.keyboardManager = keyboardManager;
            this.mouseManager = mouseManager;
            this.moveSpeed = moveSpeed;
            this.strafeSpeed = strafeSpeed;
            this.rotationSpeed = rotationSpeed;
        }

        public void Update(GameTime gameTime, IActor actor)
        {
            Actor3D parent = actor as Actor3D;

            if (parent != null)
            {
                if (this.keyboardManager.IsKeyDown(Keys.W))
                    parent.Transform3D.TranslateBy(parent.Transform3D.Look * this.moveSpeed);
                else if (this.keyboardManager.IsKeyDown(Keys.S))
                    parent.Transform3D.TranslateBy(parent.Transform3D.Look * -this.moveSpeed);

                //ASD

                if (this.keyboardManager.IsKeyDown(Keys.A))
                    parent.Transform3D.TranslateBy(parent.Transform3D.Right * -this.strafeSpeed);
                else if (this.keyboardManager.IsKeyDown(Keys.D))
                    parent.Transform3D.TranslateBy(parent.Transform3D.Right * this.strafeSpeed);

                Vector2 mouseDelta = this.mouseManager.GetDeltaFromCentre(new Vector2(512, 384));
                mouseDelta *= this.rotationSpeed * gameTime.ElapsedGameTime.Milliseconds;

                if (mouseDelta.Length() != 0)
                    parent.Transform3D.RotateBy(new Vector3(-1 * mouseDelta, 0));

            }
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

    }
}
