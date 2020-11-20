using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.GameComponents;
using Microsoft.Xna.Framework;
using System;

namespace GDLibrary.Managers
{
    public class PickingManager : PausableGameComponent
    {
        private KeyboardManager keyboardManager;
        private MouseManager mouseManager;
        private GamePadManager gamePadManager;
        private CameraManager<Camera3D> cameraManager;

        private float pickStartDistance;
        private float pickEndDistance;
        private Predicate<CollidableObject> collisionPredicate;

        public PickingManager(Game game, StatusType statusType,
            KeyboardManager keyboardManager,
            MouseManager mouseManager,
            GamePadManager gamePadManager,
            CameraManager<Camera3D> cameraManager,
           float pickStartDistance, float pickEndDistance, Predicate<CollidableObject> collisionPredicate)
           : base(game, statusType)
        {
            this.keyboardManager = keyboardManager;
            this.mouseManager = mouseManager;
            this.gamePadManager = gamePadManager;
            this.cameraManager = cameraManager;

            this.pickStartDistance = pickStartDistance;
            this.pickEndDistance = pickEndDistance;
            this.collisionPredicate = collisionPredicate;
        }

        protected override void ApplyUpdate(GameTime gameTime)
        {
            HandleMouse(gameTime);
            // base.ApplyUpdate(gameTime);
        }

        protected override void HandleMouse(GameTime gameTime)
        {
            if (this.mouseManager.IsLeftButtonClicked())
            {
                Vector3 pos, normal;
                CollidableObject pickedObject = this.mouseManager.GetPickedObject(cameraManager, 10, 1000, out pos, out normal) as CollidableObject;

                //does this object return true in the predicate
                if (this.collisionPredicate(pickedObject))
                {
                    object[] parameters = { pickedObject };
                    EventDispatcher.Publish(new EventData(EventCategoryType.Object, EventActionType.OnRemoveActor, parameters));

                    //publish other events...
                    //play sound, increment player health, start countdown, increment UI
                }
            }
            else if (this.mouseManager.IsRightButtonClicked())
            {
                Vector3 pos, normal;
                CollidableObject pickedObject = this.mouseManager.GetPickedObject(cameraManager, 10, 1000, out pos, out normal) as CollidableObject;

                //does this object return true in the predicate
                if (this.collisionPredicate(pickedObject))
                {
                    //get info about the health value of the pickup
                    int value = (pickedObject as PickupCollidableObject).Value;
                    System.Diagnostics.Debug.WriteLine(pickedObject.ID + ", " + value);
                }
            }
            //base.HandleMouse(gameTime);
        }
    }
}