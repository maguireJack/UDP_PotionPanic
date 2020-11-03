using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary.Controllers
{
    /// <summary>
    /// Implements a first person camera controller
    /// </summary>
    /// <see cref="GDLibrary.Actors.Camera3D"/>
    public class FirstPersonController : Controller
    {
        private KeyboardManager keyboardManager;
        private MouseManager mouseManager;
        private float moveSpeed, strafeSpeed, rotationSpeed;

        public FirstPersonController(string id, ControllerType controllerType,
            KeyboardManager keyboardManager,
            MouseManager mouseManager,
            float moveSpeed,
            float strafeSpeed, float rotationSpeed) : base(id, controllerType)
        {
            this.keyboardManager = keyboardManager;
            this.mouseManager = mouseManager;
            this.moveSpeed = moveSpeed;
            this.strafeSpeed = strafeSpeed;
            this.rotationSpeed = rotationSpeed;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            Actor3D parent = actor as Actor3D;

            if (parent != null)
            {
                HandleKeyboardInput(gameTime, parent);
                HandleMouseInput(gameTime, parent);
            }
        }

        private void HandleKeyboardInput(GameTime gameTime, Actor3D parent)
        {
            Vector3 moveVector = Vector3.Zero;

            if (keyboardManager.IsKeyDown(Keys.W))
            {
                moveVector = parent.Transform3D.Look * moveSpeed;
            }
            else if (keyboardManager.IsKeyDown(Keys.S))
            {
                moveVector = -1 * parent.Transform3D.Look * moveSpeed;
            }

            if (keyboardManager.IsKeyDown(Keys.A))
            {
                moveVector -= parent.Transform3D.Right * strafeSpeed;
            }
            else if (keyboardManager.IsKeyDown(Keys.D))
            {
                moveVector += parent.Transform3D.Right * strafeSpeed;
            }

            //constrain movement in Y-axis to stop object moving up/down in space
            moveVector.Y = 0;

            //apply the movement
            parent.Transform3D.TranslateBy(moveVector * gameTime.ElapsedGameTime.Milliseconds);
        }

        private void HandleMouseInput(GameTime gameTime, Actor3D parent)
        {
            Vector2 mouseDelta = mouseManager.GetDeltaFromCentre(new Vector2(512, 384));
            mouseDelta *= rotationSpeed * gameTime.ElapsedGameTime.Milliseconds;

            if (mouseDelta.Length() != 0)
            {
                parent.Transform3D.RotateBy(new Vector3(-1 * mouseDelta, 0));
            }
        }

        public new object Clone()
        {
            return new FirstPersonController(ID, ControllerType, keyboardManager,
                mouseManager, moveSpeed, strafeSpeed, rotationSpeed);
        }
    }
}