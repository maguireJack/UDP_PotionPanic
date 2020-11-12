using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using GDLibrary.Actors;
using GDLibrary.Enums;
using System.Diagnostics;
using GDGame.Game.Constants;

namespace GDGame.Game.Controllers
{
    public class ThirdPersonPlayerController : IController
    {
        #region Fields

        private KeyboardManager keyboardManager;
        private MouseManager mouseManager;
        private Camera3D camera3D;
        private float moveSpeed, strafeSpeed, rotationSpeed, turnAngle;
        private Keys[] moveKeys;
        private bool cameraMoveConstraint;
        Vector3 translateBy;

        #endregion

        #region Constructors

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
            cameraMoveConstraint = false;
            translateBy = Vector3.Zero;
        }

        #endregion

        public void Update(GameTime gameTime, IActor actor)
        {
            Actor3D parent = actor as Actor3D;

            if (parent != null)
            {
                HandleMovement(gameTime, parent);
                HandleCameraFollow(gameTime, parent);
            }
        }

        private void HandleCameraFollow(GameTime gameTime, Actor3D parent)
        {
            //Offest the objects position to where the camera should be
            Vector3 parentPos = parent.Transform3D.Translation;
            parentPos.Y += GameConstants.playerCamOffsetY;
            parentPos.Z += GameConstants.playerCamOffsetZ;

            //subtract objects position from camera position to get the distance
            parentPos -= camera3D.Transform3D.Translation;

            //Wait for the player to be slightly out of position before moving camera
            if (cameraMoveConstraint || parentPos.Length() > 70)
            {
                cameraMoveConstraint = true;
                //Offset the position before adding so it will take several updates to move to the objects position (This make the camera move smoothly)
                parentPos *= 0.01f;
                camera3D.Transform3D.Translation += parentPos;

                if (parentPos.Length() <= 0.2)
                {
                    cameraMoveConstraint = false;
                }
            }
        }

        private void HandleMovement(GameTime gameTime, Actor3D parent)
        {

            Vector3 moveVector = Vector3.Zero;

            //Move forward
            if (keyboardManager.IsKeyDown(moveKeys[0]))
                moveVector.Z -= moveSpeed;
            //Move Back
            else if (keyboardManager.IsKeyDown(moveKeys[1]))
                moveVector.Z += moveSpeed;
            //Move Left
            if (keyboardManager.IsKeyDown(moveKeys[2]))
                moveVector.X -= strafeSpeed;
            //Move Right
            else if (keyboardManager.IsKeyDown(moveKeys[3]))
                moveVector.X += strafeSpeed;


            parent.Transform3D.RotateAroundUpBy(CalculateRotation(parent, moveVector) * rotationSpeed);
            parent.Transform3D.TranslateBy(moveVector * gameTime.ElapsedGameTime.Milliseconds);
        }

        private float CalculateRotation(Actor3D parent, Vector3 moveVector)
        {
            //Convert look direction to angle in radians
            float currentAngle = (float)Math.Atan2(parent.Transform3D.Look.Z, parent.Transform3D.Look.X);

            //If moveVector not Zero, turn towards move direction
            if (moveVector != Vector3.Zero)
            {
                turnAngle = (float)Math.Atan2(moveVector.Z, moveVector.X);
                turnAngle = currentAngle - turnAngle;
            }
            else turnAngle = 0;

            if (turnAngle > Math.PI)
            {
                //if angle is bigger than 180, flip rotation
                turnAngle -= (float)(2 * Math.PI);
            }
            else if (turnAngle < -Math.PI)
            {
                //if angle is less than -180, flip rotation
                turnAngle += (float)(2 * Math.PI);
            }

            return turnAngle;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public ControllerType GetControllerType()
        {
            throw new NotImplementedException();
        }
    }
}
