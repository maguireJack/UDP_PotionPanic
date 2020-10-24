using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary
{
    public class MoveableObject : ModelObject, ICloneable
    {
        #region Fields
        //Body, wait for engine updates
        #endregion

        #region Constructors
        public MoveableObject(ModelObject modelObject, IController controller) : 
            base(modelObject)
        {
            ControllerList.Add(controller);
        }

        public MoveableObject(string id,
            ActorType actorType,
            StatusType statusType,
            Transform3D transform,
            EffectParameters effectParameters,
            Model model,
            IController controller) :
            base(id, actorType, statusType, transform, effectParameters, model) 
        {
            ControllerList.Add(controller);

        }
        #endregion
    }
}
