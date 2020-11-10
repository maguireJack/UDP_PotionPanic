using GDGame.Game.Actors;
using GDGame.Game.Interfaces;
using GDLibrary.Actors;
using Microsoft.Xna.Framework;

namespace GDGame.Game.Objects
{
    /**
     * Class for destroy items
     */
    public class Bin : InteractableActor, IContainerInteractable
    {
        #region Constructors

        public Bin(ModelObject modelObject, string name, float interactDistance) :
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
            return true;
        }
    }
}
