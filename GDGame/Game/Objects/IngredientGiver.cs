using GDGame.Game.Actors;
using GDGame.Game.Constants;
using GDGame.Game.Enums;
using GDGame.Game.Interfaces;
using GDLibrary.Actors;
using GDLibrary.Core.Events;
using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;

namespace GDGame.Game.Objects
{
    public class IngredientGiver : InteractableActor
    {
        #region Fields

        private HandHeldPickup item;

        #endregion

        #region Constructors

        public IngredientGiver(ModelObject modelObject, string name, float interactDistance, HandHeldPickup item) :
            base(modelObject, name, interactDistance)
        {
            this.item = item;
        }

        #endregion

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public HandHeldPickup TakeItem()
        {
            HandHeldPickup clone = item.Clone() as HandHeldPickup;
            EventDispatcher.Publish(EventType.Add, new ArrayList() { clone });
            return clone;
        }
    }
}
