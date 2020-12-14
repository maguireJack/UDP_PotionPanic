using GDGame.MyGame.Actors;
using Microsoft.Xna.Framework;

namespace GDGame.MyGame.Interfaces
{
    /// <summary>
    /// Allows for depositing items into minigames, cauldron, chest and trashcan
    /// </summary>
    interface IContainerInteractable
    {
        //take item from player
        bool Deposit(HandHeldPickup item);
    }
}
