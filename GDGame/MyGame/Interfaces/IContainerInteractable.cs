using GDGame.MyGame.Actors;
using Microsoft.Xna.Framework;

namespace GDGame.MyGame.Interfaces
{
    interface IContainerInteractable
    {
        //take item from player
        bool Deposit(HandHeldPickup item);
    }
}
