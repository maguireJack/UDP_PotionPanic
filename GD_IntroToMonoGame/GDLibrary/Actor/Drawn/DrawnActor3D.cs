using System;
using System.Collections.Generic;

//triangle = PrimitiveObject(vertex data) => DrawnActor3D(EffectParameters)
namespace GDLibrary
{
    public class DrawnActor3D : Actor3D
    {
        #region Fields
        private EffectParameters effectParameters;
        #endregion

        #region Properties
        public EffectParameters EffectParameters
        {
            get
            {
                return this.effectParameters;
            }
        }
        #endregion

        #region Constructors
        public DrawnActor3D(string id, ActorType actorType, StatusType statusType, Transform3D transform3D,
            EffectParameters effectParameters) : base(id, actorType, statusType, transform3D)
        {
            this.effectParameters = effectParameters;
        }
        #endregion

        public override bool Equals(object obj)
        {
            return obj is DrawnActor3D d &&
                   base.Equals(obj) &&
                   EqualityComparer<EffectParameters>.Default.Equals(effectParameters, d.EffectParameters);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), effectParameters);
        }

        public new object Clone()
        {
            return new DrawnActor3D(this.ID, this.ActorType, this.StatusType, this.Transform3D.Clone() as Transform3D,
                this.effectParameters.Clone() as EffectParameters);
        }
    }
}
