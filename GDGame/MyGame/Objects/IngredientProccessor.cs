using GDGame.MyGame.Actors;
using GDGame.MyGame.Enums;
using GDGame.MyGame.Interfaces;
using GDGame.MyGame.Objects;
using GDLibrary.Actors;
using GDLibrary.Core.Parameters.Time;
using GDLibrary.Enums;
using GDLibrary.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GDGame
{

    public class IngredientProccessor : InteractableActor, IContainerInteractable
    {
        #region Fields

        private IngredientState inputState;
        private Ingredient storedIngredient;
        private Timer timer;
        private bool timerCanStart = false;

        #endregion

        #region Properties

        public IngredientState InputState 
        { 
            get { return inputState; }
        }

        #endregion

        public IngredientProccessor(CollidableObject modelObject, string name, float interactDistance, IngredientState inputState) :
            base(modelObject, name, interactDistance)
        {
            this.inputState = inputState;
            this.timerCanStart = false;
            this.timer = new Timer(6000);

            EventDispatcher.Subscribe(EventCategoryType.Interactable, HandleEvent);
        }

        private void HandleEvent(EventData eventData)
        {
            if (eventData.EventCategoryType == EventCategoryType.Interactable)
            {
                if (eventData.EventActionType == EventActionType.OnUnlock && ((string)eventData.Parameters[0]).Equals((inputState + 1).ToString()))
                    Unlock();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            CheckComplete(gameTime);

            if (timerCanStart)
            {
                timer.StartTimer(gameTime);
                timerCanStart = false;
            }
        }

        public bool Deposit(HandHeldPickup item)
        {
            if(inputState == item.Ingredient.IngredientState)
            {
                timerCanStart = true;
                storedIngredient = item.Ingredient;
                Lock();
                return true;
            }
            return false;
        }

        private bool CheckComplete(GameTime gameTime)
        {
            if (timer.IsRunning)
            {
                if (timer.IsDone(gameTime))
                {
                    storedIngredient.Process();
                    string name = storedIngredient.IngredientType + "_" + storedIngredient.IngredientState;

                    EventDispatcher.Publish(new EventData(EventCategoryType.Pickup, 
                        EventActionType.OnProcess, new object[] { name, Transform3D.Translation }));

                    storedIngredient = null;
                    return true;
                }
            }
            return false;
        }
    }
}
