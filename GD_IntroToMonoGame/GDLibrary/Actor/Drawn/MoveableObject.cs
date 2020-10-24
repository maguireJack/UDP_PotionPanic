using GDLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary
{
    public class MoveableObject : ModelObject, ICloneable
    {
        public MoveableObject(string id,
            ActorType actorType,
            StatusType statusType,
            Transform3D transform,
            EffectParameters effectParameters,
            Model model,
            IController controller) :
            base(id, actorType, statusType, transform, effectParameters, model) 
        {

            //ControllerList.Add(controller);

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Vector3 movement = new Vector3(.1f, .1f, 0);
            this.Transform3D.TranslateBy(movement);
        }
    }
}
