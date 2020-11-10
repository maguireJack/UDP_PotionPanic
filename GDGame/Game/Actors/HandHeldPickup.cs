using GDGame.Game.Enums;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Game.Actors
{
    public class HandHeldPickup : InteractableActor
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

        public new object Clone()
        {
            HandHeldPickup actor = new HandHeldPickup(new ModelObject("clone - " + ID,
               ActorType,   //deep
               StatusType,
               Transform3D.Clone() as Transform3D,  //deep
               EffectParameters.Clone() as EffectParameters, //hybrid - shallow (texture and effect) and deep (all other fields)
               Model), ////shallow i.e. a reference)
               PickupType,
               Name,
               InteractDistance,
               heldCoords);

            //remember if we clone a model then we need to clone any attached controllers
            if (ControllerList != null)
            {
                //clone each of the (behavioural) controllers
                foreach (IController controller in ControllerList)
                {
                    actor.ControllerList.Add(controller.Clone() as IController);
                }
            }

            return actor;
        }
    }
}
