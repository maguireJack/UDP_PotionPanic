using GDGame.MyGame.Actors;

namespace GDGame.MyGame.Interfaces
{
    interface IContainerInteractable
    {
        //take item from player
        bool Deposit(HandHeldPickup item);
    }
}
