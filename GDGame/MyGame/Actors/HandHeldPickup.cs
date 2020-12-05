using GDGame.MyGame.Enums;
using GDGame.MyGame.Objects;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Interfaces;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;

namespace GDGame.MyGame.Actors
{
    public class HandHeldPickup : InteractableActor
    {
        #region Fields

        private PickupType pickupType;
        private Vector3 heldCoords;
        private Ingredient ingredient;

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

        public Ingredient Ingredient
        {
            get
            {
                return ingredient;
            }
            set
            {
                ingredient = value;
            }
        }

        #endregion

        #region Constructors

        public HandHeldPickup(CollidableObject modelObject, PickupType pickupType,
            string name, float interactDistance, Vector3 heldCoords) :
            base(modelObject, name, interactDistance)
        {
            this.pickupType = pickupType;
            this.heldCoords = heldCoords;
            this.ingredient = null;
        }

        public HandHeldPickup(CollidableObject modelObject, PickupType pickupType,
            string name, float interactDistance, Vector3 heldCoords, Ingredient ingredient) :
            base(modelObject, name, interactDistance)
        {
            this.pickupType = pickupType;
            this.heldCoords = heldCoords;
            this.ingredient = ingredient;
        }

        #endregion

        public new object Clone()
        {
            HandHeldPickup actor = new HandHeldPickup(new CollidableObject("clone - " + ID,
               ActorType,   //deep
               StatusType,
               Transform3D.Clone() as Transform3D,  //deep
               EffectParameters.Clone() as EffectParameters, //hybrid - shallow (texture and effect) and deep (all other fields)
               Model), ////shallow i.e. a reference)
               PickupType,
               Name,
               InteractDistance,
               heldCoords,
               Ingredient.Clone() as Ingredient);

            if (Locked)
                actor.Lock();

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
