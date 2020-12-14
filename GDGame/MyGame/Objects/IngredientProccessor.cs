using GDGame.MyGame.Actors;
using GDGame.MyGame.Constants;
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
        private IngredientState outputState;
        private Ingredient storedIngredient;
        private Minigame minigame;
        private Timer timer;
        private string sound = "ping";

        #endregion

        #region Properties

        public IngredientState InputState 
        { 
            get { return inputState; }
        }

        #endregion

        public IngredientProccessor(CollidableObject modelObject, string name, float interactDistance,
            IngredientState inputState, IngredientState outputState, Minigame minigame)
            : base(modelObject, name, interactDistance)
        {
            this.inputState = inputState;
            this.outputState = outputState;
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
                    EventDispatcher.Publish(new EventData(EventCategoryType.Sound, EventActionType.OnPlay, new object[] { sound }));

                    storedIngredient.IngredientState = outputState;
                    string name = storedIngredient.IngredientType + "_" + storedIngredient.IngredientState;

                    storedIngredient.Score = (int)(GameConstants.minigameScore * CalculatePercentageScore(timer.ElapsedTime));

                    //Send event to add to object manager
                    EventDispatcher.Publish(new EventData(EventCategoryType.Pickup,
                        EventActionType.OnProcess, new object[] { name, Transform3D.Translation }));

                    storedIngredient = null;
                }
                else minigame.Update(gameTime);
            }
            base.Update(gameTime);
        }

        private double CalculatePercentageScore(double time)
        {
            if (time > 12000)       //20% of score
                return 20f / 100f;
            else if (time > 8000)   //50% of score
                return 50f / 100f;
            else if (time > 6000)   //70% of score
                return 70f / 100f;
            return 1;               //100% of score
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
