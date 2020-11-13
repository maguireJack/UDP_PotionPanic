using GDGame.Game.Actors;
using GDGame.Game.Constants;
using GDGame.Game.Enums;
using GDGame.Game.Interfaces;
using GDLibrary.Actors;
using GDLibrary.Core.Events;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GDGame.Game.Objects
{
    public class Cauldron : InteractableActor, IContainerInteractable
    {
        #region Fields

        private Recipe inventory;

        #endregion

        #region Constructors

        public Cauldron(ModelObject modelObject, string name, float interactDistance, EventDispatcher eventDispatcher) :
            base(modelObject, name, interactDistance)
        {
            inventory = new Recipe();
            eventDispatcher.PotionPickedEvent += EventDispatcher_PotionPickedEvent;
        }

        private void EventDispatcher_PotionPickedEvent()
        {
            Unlock();
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
                        EventDispatcher.Publish(EventType.Recipe, GameConstants.potions[key]);
                        Lock(); //Lock the cauldron so the player cannot put items in until the potion is taken away 
                        break;
                    }
                }
                inventory.Clear();
            }
        }
    }
}
