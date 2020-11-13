using GDGame.MyGame.Actors;
using GDGame.MyGame.Constants;
using GDGame.MyGame.Enums;
using GDGame.MyGame.Interfaces;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using Microsoft.Xna.Framework;

namespace GDGame.MyGame.Objects
{
    public class Cauldron : InteractableActor, IContainerInteractable
    {
        #region Fields

        private Recipe inventory;

        #endregion

        #region Constructors

        public Cauldron(ModelObject modelObject, string name, float interactDistance) :
            base(modelObject, name, interactDistance)
        {
            inventory = new Recipe();
            EventDispatcher.Subscribe(EventCategoryType.Interactable, HandleEvent);
        }

        private void HandleEvent(EventData eventData)
        {
            if(eventData.EventCategoryType == EventCategoryType.Interactable)
            {
                if (eventData.EventActionType == EventActionType.OnUnlock)
                    Unlock();
            }
        }

        #endregion

        public override void Update(GameTime gameTime)
        {
            Recipes();
            base.Update(gameTime);
        }

        public bool Deposit(HandHeldPickup item)
        {
            switch(item.PickupType)
            {
                case PickupType.Ingredient:
                    Add(item.Ingredient);
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

        private void Recipes()
        {
            int count = 0;
            foreach(int value in inventory.Ingredients.Values)
            {
                count += value;
            }
            if(count > 2)
            {
                //for each key (recipe) check to see if the inventory of the cauldron matches the recipe
                foreach (Recipe key in GameConstants.potions.Keys)
                {
                    if (inventory.Equals(key))
                    {
                        //If it does, get the data of the potion and dispatch an event
                        EventDispatcher.Publish(new EventData(EventCategoryType.Pickup,
                            EventActionType.OnCreate, new object[] { GameConstants.potions[key] }));
                        Lock(); //Lock the cauldron so the player cannot put items in until the potion is taken away 
                        break;
                    }
                }
                inventory.Clear();
            }
        }
    }
}
