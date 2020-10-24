using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using GDLibrary;

namespace GDGame
{
    public class ThirdPersonPlayerController : IController
    {
        private KeyboardManager keyboardManager;
        private MouseManager mouseManager;
        private Camera3D camera3D;
        private float moveSpeed, strafeSpeed, rotationSpeed;
        private Keys[] moveKeys;

        public ThirdPersonPlayerController(KeyboardManager keyboardManager,
            MouseManager mouseManager,
            Camera3D camera3D,
            float moveSpeed, float strafeSpeed, float rotationSpeed,
            Keys[] moveKeys
            )
        {
            this.keyboardManager = keyboardManager;
            this.mouseManager = mouseManager;
            this.camera3D = camera3D;
            this.moveSpeed = moveSpeed;
            this.strafeSpeed = strafeSpeed;
            this.rotationSpeed = rotationSpeed;
            this.moveKeys = moveKeys;
        }

        public void Update(GameTime gameTime, IActor actor)
        {
            Actor3D parent = actor as Actor3D;

            if (parent != null)
            {
                HandleInput(gameTime, parent);
                HandleCameraFollow(gameTime, parent);
            }
        }

        private void HandleInput(GameTime gameTime, Actor3D parent)
        {
            HandleKeyboardInput(gameTime, parent);
            HandleMouseInput(gameTime, parent);
            HandleCameraFollow(gameTime, parent);
        }

        private void HandleCameraFollow(GameTime gameTime, Actor3D parent)
        {

        }

        private void HandleKeyboardInput(GameTime gameTime, Actor3D parent)
        {
            Vector3 moveVector = Vector3.Zero;

            if (this.keyboardManager.IsKeyDown(moveKeys[0]))
                moveVector = parent.Transform3D.Look * this.moveSpeed;
            else if (this.keyboardManager.IsKeyDown(moveKeys[1]))
                moveVector = -1 * parent.Transform3D.Look * this.moveSpeed;

            if (this.keyboardManager.IsKeyDown(moveKeys[2]))
                moveVector -= parent.Transform3D.Right * this.strafeSpeed;
            else if (this.keyboardManager.IsKeyDown(moveKeys[3]))
                moveVector += parent.Transform3D.Right * this.strafeSpeed;

            //constrain movement in Y-axis
            moveVector.Y = 0;

            parent.Transform3D.TranslateBy(moveVector * gameTime.ElapsedGameTime.Milliseconds);
        }

        private void HandleMouseInput(GameTime gameTime, Actor3D parent)
        {
            Vector2 mouseDelta = this.mouseManager.GetDeltaFromCentre(new Vector2(512, 384));
            mouseDelta *= this.rotationSpeed * gameTime.ElapsedGameTime.Milliseconds;

            if (mouseDelta.Length() != 0)
                parent.Transform3D.RotateBy(new Vector3(-1 * mouseDelta, 0));

        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

    }
}
