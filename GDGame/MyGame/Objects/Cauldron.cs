using GDGame.MyGame.Actors;
using GDGame.MyGame.Constants;
using GDGame.MyGame.Controllers;
using GDGame.MyGame.Enums;
using GDGame.MyGame.Interfaces;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;

namespace GDGame.MyGame.Objects
{
    public class Cauldron : InteractableActor, IContainerInteractable
    {
        #region Fields

        private Recipe inventory;
        private StirringMinigameController minigame;
        private Timer timer;
        private Recipe key;

        #endregion

        #region Constructors

        public Cauldron(CollidableObject collidableObject, string name, float interactDistance,
            StirringMinigameController minigame)
            : base(collidableObject, name, interactDistance)
        {
            inventory = new Recipe();
            this.minigame = minigame;
            this.key = null;
            this.timer = new Timer();

            EventDispatcher.Subscribe(EventCategoryType.Interactable, HandleEvent);
        }

        private void HandleEvent(EventData eventData)
        {
            if(eventData.EventCategoryType == EventCategoryType.Interactable)
            {
                if (eventData.EventActionType == EventActionType.OnUnlock && ((string)eventData.Parameters[0]).Equals("Cauldron"))
                    Unlock();
            }
        }

        #endregion

        public override void Update(GameTime gameTime)
        {
            

            if((minigame.StatusType & StatusType.Update) == StatusType.Update)
            {
                if (!timer.IsRunning)
                {
                    timer.StartTimer(gameTime);
                }

                if (minigame.IsComplete())
                {
                    timer.StopTimer(gameTime);

                    //Send score event
                    EventDispatcher.Publish(new EventData(EventCategoryType.Player,
                        EventActionType.OnMinigameStir, new object[] { timer.ElapsedTime, GameConstants.potions[key][0] as string }));

                    //Send event to create potion and add to object manager
                    EventDispatcher.Publish(new EventData(EventCategoryType.Pickup,
                        EventActionType.OnCreate, new object[] { GameConstants.potions[key] }));
                }
                else minigame.Update(gameTime);
            }
            base.Update(gameTime);
        }

        public bool Deposit(HandHeldPickup item)
        {
            switch(item.PickupType)
            {
                case PickupType.Ingredient:
                    Add(item.Ingredient);
                    CheckRecipes();
                    break;
                default:
                    return false;
            }

            return true;
        }

        private void Add(Ingredient item)
        {
            if(inventory.ContainsKey(item))
            {
                inventory.Ingredients[item]++;
            }
            else inventory.Add(item, 1);
        }

        private void CheckRecipes()
        {
            //for each key (recipe) check to see if the inventory of the cauldron matches the recipe
            foreach (Recipe key in GameConstants.potions.Keys)
            {
                if (inventory.Equals(key))
                {
                    //start minigame
                    minigame.Start();

                    //save key for when minigame is complete
                    this.key = key;

                    //Lock the cauldron so the player cannot put items in until the potion is taken away 
                    Lock();
                    inventory.Clear();
                    break;
                }
            }
        }
    }
}
