using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary.Controllers
{
    /// <summary>
    /// Implements a flight person camera controller which uses keyboard and mouse input
    /// </summary>
    /// <see cref="GDLibrary.Actors.Camera3D"/>
    /// <seealso cref="GDLibrary.Parameters.MoveParameters"/>
    public class FlightCameraController : Controller
    {
        #region Fields

        private MoveParameters moveParameters;

        #endregion Fields

        #region Constructors & Core

        public FlightCameraController(string id, ControllerType controllerType,
            MoveParameters moveParameters) : base(id, controllerType)
        {
            this.moveParameters = moveParameters;
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

        #region Movement

        private void HandleKeyboardInput(GameTime gameTime, Actor3D parent)
        {
            if (moveParameters.KeyboardManager.IsKeyDown(Keys.W))
            {
                parent.Transform3D.TranslateBy(parent.Transform3D.Look * moveParameters.MoveSpeed);
            }
            else if (moveParameters.KeyboardManager.IsKeyDown(Keys.S))
            {
                parent.Transform3D.TranslateBy(parent.Transform3D.Look * -moveParameters.MoveSpeed);
            }

            if (moveParameters.KeyboardManager.IsKeyDown(Keys.A))
            {
                parent.Transform3D.TranslateBy(parent.Transform3D.Right * -moveParameters.StrafeSpeed);
            }
            else if (moveParameters.KeyboardManager.IsKeyDown(Keys.D))
            {
                parent.Transform3D.TranslateBy(parent.Transform3D.Right * moveParameters.StrafeSpeed);
            }
        }

        private void HandleMouseInput(GameTime gameTime, Actor3D parent)
        {
            Vector2 mouseDelta = moveParameters.MouseManager.GetDeltaFromCentre(new Vector2(512, 384));
            mouseDelta *= moveParameters.RotateSpeed * gameTime.ElapsedGameTime.Milliseconds;

            if (mouseDelta.Length() != 0)
            {
                parent.Transform3D.RotateBy(new Vector3(-1 * mouseDelta, 0));
            }
        }

        #endregion Movement

        public new object Clone()
        {
            return new FlightCameraController(ID, ControllerType, moveParameters.Clone() as MoveParameters);
        }

        #endregion Constructors & Core
    }
}