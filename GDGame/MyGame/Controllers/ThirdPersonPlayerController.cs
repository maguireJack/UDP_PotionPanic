using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDGame.MyGame.Constants;
using GDLibrary.Events;
using GDLibrary.Controllers;
using GDLibrary;

namespace GDGame.MyGame.Controllers
{
    public class ThirdPersonPlayerController : Controller
    {
        #region Fields

        private KeyboardManager keyboardManager;
        private MouseManager mouseManager;
        private Camera3D camera3D;
        private float moveSpeed, rotationSpeed, originalMoveSpeed;
        private Keys[][] moveKeys;
        private bool cameraMoveConstraint;
        private bool locked;

        #endregion

        #region Constructors

        public ThirdPersonPlayerController(string id, ControllerType controllerType,
            KeyboardManager keyboardManager,
            MouseManager mouseManager,
            Camera3D camera3D,
            float moveSpeed, float rotationSpeed,
            Keys[][] moveKeys)
            : base(id, controllerType)
        {
            this.keyboardManager = keyboardManager;
            this.mouseManager = mouseManager;
            this.camera3D = camera3D;
            this.moveSpeed = this.originalMoveSpeed = moveSpeed;
            this.rotationSpeed = rotationSpeed;
            this.moveKeys = moveKeys;
            this.cameraMoveConstraint = false;
            this.locked = false;

            EventDispatcher.Subscribe(EventCategoryType.Upgrade, HandleEvent);
            EventDispatcher.Subscribe(EventCategoryType.Player, HandleEvent);
        }

        #endregion

        private void HandleEvent(EventData eventData)
        {
            if(eventData.EventCategoryType == EventCategoryType.Player)
            {
                if(eventData.EventActionType == EventActionType.OnLock)
                {
                    locked = true;
                }
                if (eventData.EventActionType == EventActionType.OnUnlock)
                {
                    locked = false;
                }
            }
            else if (eventData.EventCategoryType == EventCategoryType.Upgrade)
            {
                if (eventData.EventActionType == EventActionType.MoveSpeedUp)
                {
                    moveSpeed = originalMoveSpeed + (((float)eventData.Parameters[0]) / 100f * originalMoveSpeed);
                }
            }
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            //checks if controller is connected
            GamePadCapabilities capabilities = GamePad.GetCapabilities(PlayerIndex.One);

            Actor3D parent = actor as Actor3D;

            if (parent != null && !locked)
            {
                if (capabilities.IsConnected)
                {
                    HandleControlerMovement(gameTime, parent, capabilities);
                }
                else
                {
                    HandleMovement(gameTime, parent);
                }
                HandleCameraFollow(gameTime, parent);
            }
            else
            {
                EventDispatcher.Publish(new EventData(EventCategoryType.Sound, EventActionType.OnPause, new object[] { "walking" }));
            }
                

            base.Update(gameTime, actor);
        }

        private void HandleControlerMovement(GameTime gameTime, Actor3D parent, GamePadCapabilities capabilities)
        {
            CharacterObject character = parent as CharacterObject;
            Vector3 moveVector = Vector3.Zero;
            GamePadState state = GamePad.GetState(PlayerIndex.One);
            if (capabilities.HasLeftXThumbStick)
            {
                moveVector.X = state.ThumbSticks.Left.X * moveSpeed;
                moveVector.Z = -state.ThumbSticks.Left.Y * moveSpeed;
            }
            parent.Transform3D.RotateAroundUpBy(MathUtility.CalculateRotationToVector(parent.Transform3D.Look, moveVector) * rotationSpeed);
            character.CharacterBody.Velocity += moveVector * gameTime.ElapsedGameTime.Milliseconds;
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
            CharacterObject character = parent as CharacterObject;
            Vector3 moveVector = Vector3.Zero;
            

            //Move forward
            if (keyboardManager.IsAnyKeyPressed(moveKeys, 0))
                moveVector.Z -= moveSpeed;
            //Move Back
            else if (keyboardManager.IsAnyKeyPressed(moveKeys, 1))
                moveVector.Z += moveSpeed;
            //Move Left
            if (keyboardManager.IsAnyKeyPressed(moveKeys, 2))
                moveVector.X -= moveSpeed;
            //Move Right
            else if (keyboardManager.IsAnyKeyPressed(moveKeys, 3))
                moveVector.X += moveSpeed;

           

            if (moveVector == Vector3.Zero)
                character.CharacterBody.DesiredVelocity = Vector3.Zero;

            if (moveVector != Vector3.Zero)
            {
                EventDispatcher.Publish(new EventData(EventCategoryType.Sound, EventActionType.OnPlay, new object[] { "walking" }));
            }
            else
                EventDispatcher.Publish(new EventData(EventCategoryType.Sound, EventActionType.OnPause, new object[] { "walking" }));

            parent.Transform3D.RotateAroundUpBy(MathUtility.CalculateRotationToVector(parent.Transform3D.Look, moveVector) * rotationSpeed);
            character.CharacterBody.Velocity += moveVector * gameTime.ElapsedGameTime.Milliseconds;
        }

        public new object Clone()
        {
            throw new NotImplementedException();
        }

        public new ControllerType GetControllerType()
        {
            throw new NotImplementedException();
        }
    }
}
