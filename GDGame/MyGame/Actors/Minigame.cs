using GDLibrary.Actors;
using GDLibrary.Enums;

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
            StatusType = StatusType.Drawn | StatusType.Update;
        }

        public virtual bool IsComplete()
        {
            StatusType = StatusType.Off;
            return true;
        }
    }
}
