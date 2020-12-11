using GDGame.MyGame.Actors;
using GDGame.MyGame.Enums;
using GDGame.MyGame.Interfaces;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;

namespace GDGame.MyGame.Objects
{

    public class IngredientProccessor : InteractableActor, IContainerInteractable
    {
        #region Fields

        private IngredientState inputState;
        private Ingredient storedIngredient;
        private Minigame minigame;
        private Timer timer;

        #endregion

        #region Properties

        public IngredientState InputState 
        { 
            get { return inputState; }
        }

        #endregion

        public IngredientProccessor(CollidableObject modelObject, string name, float interactDistance,
            IngredientState inputState, Minigame minigame)
            : base(modelObject, name, interactDistance)
        {
            this.inputState = inputState;
            this.minigame = minigame;
            this.timer = new Timer();

            EventDispatcher.Subscribe(EventCategoryType.Interactable, HandleEvent);
        }

        private void HandleEvent(EventData eventData)
        {
            if (eventData.EventCategoryType == EventCategoryType.Interactable)
            {
                if (eventData.EventActionType == EventActionType.OnUnlock
                    && ((string)eventData.Parameters[0]).Equals((inputState + 1).ToString()))
                {
                    Unlock();
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if ((minigame.StatusType & StatusType.Update) == StatusType.Update)
            {
                if (!timer.IsRunning)
                {
                    timer.StartTimer(gameTime);
                }

                if (minigame.IsComplete())
                {
                    timer.StopTimer(gameTime);
                    StatusType = StatusType.Drawn;

                    storedIngredient.Process();
                    string name = storedIngredient.IngredientType + "_" + storedIngredient.IngredientState;

                    //Send score event
                    EventDispatcher.Publish(new EventData(EventCategoryType.Player,
                        EventActionType.OnMinigameGrind, new object[] { timer.ElapsedTime, storedIngredient }));

                    //Send event to add to object manager
                    EventDispatcher.Publish(new EventData(EventCategoryType.Pickup,
                        EventActionType.OnProcess, new object[] { name, Transform3D.Translation }));

                    storedIngredient = null;
                }
                else minigame.Update(gameTime);
            }
            base.Update(gameTime);
        }

        public bool Deposit(HandHeldPickup item)
        {
            if(inputState == item.Ingredient.IngredientState)
            {
                StatusType = StatusType.Drawn | StatusType.Update;
                minigame.Start();
                storedIngredient = item.Ingredient;
                Lock();
                return true;
            }
            return false;
        }
    }
}
