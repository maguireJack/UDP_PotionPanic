using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Events;

namespace GDGame.MyGame.Actors
{
    public class Minigame : Actor
    {
        public Minigame(string id, ActorType actorType, StatusType statusType)
            : base(id, actorType, statusType)
        {
        }

        public virtual void Start()
        {
            StatusType = StatusType.Update;
        }

        public virtual bool IsComplete()
        {
            StatusType = StatusType.Off;
            return true;
        }

        public virtual void SendLockEvent()
        {
            EventDispatcher.Publish(new EventData(EventCategoryType.Player,
                EventActionType.OnLock, null));
        }

        public virtual void SendUnlockEvent()
        {
            EventDispatcher.Publish(new EventData(EventCategoryType.Player,
                EventActionType.OnUnlock, null));
        }
    }
}
