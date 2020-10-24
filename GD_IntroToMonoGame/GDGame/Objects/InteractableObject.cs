using GDLibrary;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame
{
    public class InteractableObject : ModelObject
    {
        #region Fields
        private string name;
        private float interactDistance;
        #endregion

        #region Properties
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public float InteractDistance
        {
            get
            {
                return this.interactDistance;
            }
        }
        #endregion

        #region Constructors
        public InteractableObject(ModelObject modelObject,
            string name, float interactDistance) : base(modelObject)
        {
            this.name = name;
            this.interactDistance = interactDistance;
        }

        public InteractableObject(string id,
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
        }
        #endregion
    }
}
