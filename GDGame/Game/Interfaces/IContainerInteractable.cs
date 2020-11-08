using GDGame.Game.Actors;

namespace GDGame.Game.Interfaces
{
    interface IContainerInteractable
    {
        //take item from player
        bool Deposit(HandHeldPickup item);
    }
}
