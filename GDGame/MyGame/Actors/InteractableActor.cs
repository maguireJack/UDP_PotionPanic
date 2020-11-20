using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.MyGame.Actors
{
    public abstract class InteractableActor : CollidableObject
    {
        #region Fields

        private string name;
        private float interactDistance;
        private bool locked;

        #endregion

        #region Properties

        public string Name
        {
            get
            {
                return name;
            }
        }

        public float InteractDistance
        {
            get
            {
                return interactDistance;
            }
        }

        public bool Locked
        {
            get
            {
                return locked;
            }
        }

        #endregion

        #region Constructors

        public InteractableActor(ModelObject modelObject,
            string name, float interactDistance) : base(modelObject)
        {
            this.name = name;
            this.interactDistance = interactDistance;
            locked = false;
        }

        public InteractableActor(string id,
            ActorType actorType,
            StatusType statusType,
            Transform3D transform,
            EffectParameters effectParameters,
            Model model,
            string name,
            float interactDistance) :
            base(id, actorType, statusType, transform, effectParameters, model)
        {
            this.name = name;
            this.interactDistance = interactDistance;
            locked = false;
        }

        #endregion

        public float GetDistance(Actor3D actor)
        {
            Vector2 actorTranslation = new Vector2(actor.Transform3D.Translation.X, actor.Transform3D.Translation.Z);
            Vector2 thisTranslation = new Vector2(Transform3D.Translation.X, Transform3D.Translation.Z);
            return Vector2.Distance(actorTranslation, thisTranslation);
        }

        protected void Lock()
        {
            locked = true;
        }

        protected void Unlock()
        {
            locked = false;
        }
    }
}
