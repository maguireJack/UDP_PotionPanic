﻿using GDGame.MyGame.Actors;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;
using Microsoft.Xna.Framework;

namespace GDGame.MyGame.Objects
{
    public class IngredientGiver : InteractableActor
    {
        #region Fields

        private HandHeldPickup item;

        #endregion

        #region Constructors

        public IngredientGiver(CollidableObject modelObject, string name, float interactDistance, HandHeldPickup item) :
            base(modelObject, name, interactDistance)
        {
            this.item = item;
        }

        #endregion

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        /// <summary>
        /// Gives player a handheld pickup
        /// </summary>
        /// <returns> A handheld pickup that  is placed in the player</returns>
        public HandHeldPickup TakeItem()
        {
            HandHeldPickup clone = item.Clone() as HandHeldPickup;
            //Add item to object manager
            EventDispatcher.Publish(new EventData(EventCategoryType.Object,
                EventActionType.OnAddActor, new object[] { clone }));
            EventDispatcher.Publish(new EventData(EventCategoryType.Sound, EventActionType.OnPlay, new object[] { "grabbing" }));
            //give it to player
            return clone;
        }
    }
}
