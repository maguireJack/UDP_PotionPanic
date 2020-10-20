using System;
using System.Collections.Generic;

namespace GDLibrary
{
    public class Actor3D : Actor
    {
        #region Fields
        private Transform3D transform3D;
        #endregion

        #region Properties
        public Transform3D Transform3D
        {
            get
            {
                return this.transform3D;
            }
            set
            {
                this.transform3D = value;
            }
        }
        #endregion

        #region Constructors
        public Actor3D(string id, ActorType actorType, StatusType statusType, Transform3D transform3D) : base(id, actorType, statusType)
        {
            this.transform3D = transform3D;
        }
        #endregion

        public override bool Equals(object obj)
        {
            return obj is Actor3D d &&
                   base.Equals(obj) &&
                   EqualityComparer<Transform3D>.Default.Equals(transform3D, d.transform3D);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), transform3D);
        }

        public new object Clone()
        {
            //deep-copy
            return new Actor3D(this.ID, this.ActorType, this.StatusType, this.transform3D.Clone() as Transform3D);
        }
    }
}
