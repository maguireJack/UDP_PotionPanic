using System;

namespace GDLibrary.Parameters
{
    /// <summary>
    /// Encapsulates the trig specific parameters (e.g. y = A*Sin(w*T+phi) for use with a IController object
    /// </summary>
    /// <see cref="GDLibrary.Controllers.PanController"/>
    public class TrigonometricParameters : ICloneable
    {
        #region Fields

        private float maxAmplitude, angularSpeed, phaseAngle;

        #endregion Fields

        #region Properties
        public float MaxAmplitude { get => maxAmplitude; set => maxAmplitude = value; }
        public float AngularSpeed { get => angularSpeed; set => angularSpeed = value; }
        public float PhaseAngle { get => phaseAngle; set => phaseAngle = value; }
        #endregion Properties

        #region Constructors & Core

        public TrigonometricParameters(float maxAmplitude, float angularSpeed, float phaseAngle)
        {
            MaxAmplitude = maxAmplitude;
            AngularSpeed = angularSpeed;
            PhaseAngle = phaseAngle;
        }

        public object Clone()
        {
            return new TrigonometricParameters(MaxAmplitude, AngularSpeed, PhaseAngle);
        }

        #endregion Constructors & Core
    }
}