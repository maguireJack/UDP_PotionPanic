using GDLibrary.Enums;
using GDLibrary.Parameters;
using System;
using System.Collections.Generic;

namespace GDLibrary.Actors
{
    /// <summary>
    /// Base class for all drawn and undrawn 3D actors used in the engine. This class adds a Transform3D field.
    /// </summary>
    public class Actor3D : Actor
    {
        #region Fields

        private Transform3D transform3D;

        #endregion Fields

        #region Properties

        public Transform3D Transform3D
        {
            get
            {
                return transform3D;
            }
            set
            {
                transform3D = value;
            }
        }

        #endregion Properties

        #region Constructors

        public Actor3D(string id, ActorType actorType, StatusType statusType, Transform3D transform3D) : base(id, actorType, statusType)
        {
            this.transform3D = transform3D;
        }

        #endregion Constructors

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
            return new Actor3D(ID, ActorType, StatusType, transform3D.Clone() as Transform3D);
        }
    }
}