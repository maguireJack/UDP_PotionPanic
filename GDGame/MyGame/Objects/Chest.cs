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
    public class Chest : InteractableActor, IContainerInteractable
    {
        private PrimitiveObject helper;
        private Timer timer;

        public Chest(CollidableObject modelObject, string name, float interactDistance,
            PrimitiveObject helper)
            : base(modelObject, name, interactDistance)
        {
            this.helper = helper;
            this.timer = new Timer(1000);
            EventDispatcher.Subscribe(EventCategoryType.Player, HandleEvent);
        }

        private void HandleEvent(EventData eventData)
        {
            if(eventData.EventCategoryType == EventCategoryType.Player)
            {
                if(eventData.EventActionType == EventActionType.OnPickup)
                {
                    StatusType = StatusType.Drawn | StatusType.Update;
                    helper.StatusType = StatusType.Drawn;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!timer.IsRunning)
            {
                timer.StartTimer(gameTime);
            }

            if (timer.IsDone(gameTime))
            {
                if(helper.StatusType == StatusType.Drawn)
                {
                    helper.StatusType = StatusType.Off;
                    timer.TimerLengthMS = 500;
                    timer.StartTimer(gameTime);
                }
                else
                {
                    helper.StatusType = StatusType.Drawn;
                    timer.TimerLengthMS = 1000;
                    timer.StartTimer(gameTime);
                }
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// Takes the potion from player
        /// </summary>
        /// <param name="item">potion</param>
        /// <returns>true if the player is putting a potion into the chest</returns>
        public bool Deposit(HandHeldPickup item)
        {
            if(item.PickupType == PickupType.Potion)
            {
                helper.StatusType = StatusType.Off;
                StatusType = StatusType.Drawn;

                //publish event to cauldron to give new recipe
                EventDispatcher.Publish(new EventData(EventCategoryType.Player,
                    EventActionType.OnPotionDeposit, null));
                return true;
            }
            return false;
        }
    }
}
