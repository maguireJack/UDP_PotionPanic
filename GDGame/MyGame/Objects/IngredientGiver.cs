using GDGame.MyGame.Actors;
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
            //Add item to object manager
            EventDispatcher.Publish(new EventData(EventCategoryType.Object,
                EventActionType.OnAddActor, new object[] { clone }));
            //give it to player
            return clone;
        }
    }
}
