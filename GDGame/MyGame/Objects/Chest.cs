﻿using GDGame.MyGame.Actors;
using GDGame.MyGame.Enums;
using GDGame.MyGame.Interfaces;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;

namespace GDGame.MyGame.Objects
{
    public class Chest : InteractableActor, IContainerInteractable
    {
        public Chest(CollidableObject modelObject, string name, float interactDistance)
            : base(modelObject, name, interactDistance)
        {

        }

        public bool Deposit(HandHeldPickup item)
        {
            if(item.PickupType == PickupType.Potion)
            {
                //publish event to cauldron to give new recipe
                EventDispatcher.Publish(new EventData(EventCategoryType.Player,
                    EventActionType.OnPotionDeposit, null));
                return true;
            }
            return false;
        }
    }
}
