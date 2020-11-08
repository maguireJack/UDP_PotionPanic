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
    public class Cauldron : InteractableObject, IContainerInteractable
    {
        #region Fields

        private HashSet<string> inventory;

        #endregion

        #region Constructors

        public Cauldron(ModelObject modelObject, string name, float interactDistance) :
            base(modelObject, name, interactDistance)
        {
            inventory = new HashSet<string>();
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
                        inventory.Clear();

                        //If it does, get the data of the potion and dispatch an event
                        EventDispatcher.Publish(EventType.Recipe, GameConstants.potions[key]);
                        return;
                    }
                }
            }
        }
    }
}
