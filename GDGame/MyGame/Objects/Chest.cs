using GDGame.MyGame.Actors;
using GDGame.MyGame.Enums;
using GDGame.MyGame.Interfaces;
using GDLibrary.Actors;

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
                return true;
            }
            return false;
        }
    }
}
