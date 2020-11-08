using GDGame.Game.Enums;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Game.Actors
{
    public class HandHeldPickup : InteractableObject
    {
        #region Fields

        private PickupType pickupType;
        private Vector3 heldCoords;

        #endregion

        #region Properties

        public PickupType PickupType
        {
            get
            {
                return pickupType;
            }
        }

        public Vector3 HeldCoords
        {
            get
            {
                return heldCoords;
            }
        }

        #endregion

        #region Constructors

        public HandHeldPickup(ModelObject modelObject, PickupType pickupType,
            string name, float interactDistance, Vector3 heldCoords) :
            base(modelObject, name, interactDistance)
        {
            this.pickupType = pickupType;
            this.heldCoords = heldCoords;
        }

        public HandHeldPickup(string id,
            ActorType actorType,
            StatusType statusType,
            Transform3D transform,
            EffectParameters effectParameters,
            Model model,
            PickupType pickupType,
            string name,
            float interactDistance,
            Vector3 heldCoords) :
            base(id, actorType, statusType, transform, effectParameters, model,
                name, interactDistance)
        {
            this.pickupType = pickupType;
            this.heldCoords = heldCoords;
        }

        #endregion
    }
}
