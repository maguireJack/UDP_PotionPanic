using GDGame.Game.Actors;
using GDGame.Game.Constants;
using GDGame.Game.Enums;
using GDGame.Game.Interfaces;
using GDLibrary.Actors;
using GDLibrary.Core.Events;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;

namespace GDGame.Game.Objects
{
    public class Cauldron : InteractableActor, IContainerInteractable
    {
        #region Fields

        private HashSet<string> inventory;

        #endregion

        #region Constructors

        public Cauldron(ModelObject modelObject, string name, float interactDistance, EventDispatcher eventDispatcher) :
            base(modelObject, name, interactDistance)
        {
            inventory = new HashSet<string>();
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
                case PickupType.Solid:
                    Add(item.Name, 1);
                    break;
                case PickupType.Dust:
                    Add(item.Name, 1);
                    break;
                case PickupType.Liquid:
                    Add(item.Name, 1);
                    break;
                default:
                    return false;
            }

            return true;
        }

        private void Add(string name, int n)
        {
            if (inventory.Contains(name + n))
            {
                Add(name, n + 1);
            }
            else inventory.Add(name + n);
        }

        private void Recipes()
        {
            if(inventory.Count > 2)
            {
                //for each key (recipe) check to see if the inventory of the cauldron matches the recipe
                foreach(HashSet<string> key in GameConstants.potions.Keys)
                {
                    if(inventory.SetEquals(key))
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
