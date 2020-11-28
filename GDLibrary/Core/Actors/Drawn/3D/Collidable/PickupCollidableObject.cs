using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary.Actors
{
    public class PickupCollidableObject : CollidableObject
    {
        private int value;

        public int Value { get => value; set => this.value = value; }

        public PickupCollidableObject(string id, ActorType actorType, StatusType statusType,
            Transform3D transform, EffectParameters effectParameters, Model model, int value)
            : base(id, actorType, statusType, transform, effectParameters, model)
        {
            Value = value;
        }

        public new object Clone()
        {
            return new PickupCollidableObject("clone - " + ID, //deep
                ActorType,   //deep
                StatusType,
                Transform3D.Clone() as Transform3D,  //deep
                EffectParameters.Clone() as EffectParameters, //hybrid - shallow (texture and effect) and deep (all other fields)
                Model,
                this.value); //shallow i.e. a reference
        }
    }
}