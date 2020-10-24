using GDLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame
{
    public class HandHeldPickup : InteractableObject
    {
        #region Fields
        private Vector3 heldCoords;
        #endregion

        #region Properties
        public Vector3 HeldCoords
        {
            get
            {
                return this.heldCoords;
            }
        }
        #endregion

        #region Constructors
        public HandHeldPickup(ModelObject modelObject,
            string name, float interactDistance, Vector3 heldCoords) :
            base(modelObject, name, interactDistance)
        {
            this.heldCoords = heldCoords;
        }

        public HandHeldPickup(string id,
            ActorType actorType,
            StatusType statusType,
            Transform3D transform,
            EffectParameters effectParameters,
            Model model,
            string name,
            float interactDistance,
            Vector3 heldCoords) :
            base(id, actorType, statusType, transform, effectParameters, model,
                name, interactDistance)
        {
            this.heldCoords = heldCoords;
        }
        #endregion
    }
}
