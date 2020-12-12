﻿using GDGame.MyGame.Actors;
using GDGame.MyGame.Enums;
using GDGame.MyGame.Interfaces;
using GDLibrary.Actors;
using Microsoft.Xna.Framework;

namespace GDGame.MyGame.Objects
{
    /**
     * Class for destroy items
     */
    public class Bin : InteractableActor, IContainerInteractable
    {
        #region Constructors

        public Bin(CollidableObject modelObject, string name, float interactDistance) :
            base(modelObject, name, interactDistance)
        {

        }

        #endregion

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public bool Deposit(HandHeldPickup item)
        {
            if (item.PickupType == PickupType.Potion)
                return false;
            return true;
        }
    }
}
