using Microsoft.Xna.Framework;

namespace GDLibrary.Parameters
{
    /// <summary>
    /// Allows the developer to define two curves with two independent values changing over time (e.g. (x,y) vs time, (height, width) vs time) and then evaluate the (x,y) quantity for any valid time on that curve
    /// </summary>
    public class Curve2D
    {
        #region Fields

        private Curve1D xCurve, yCurve;
        private CurveLoopType curveLookType;

        #endregion Fields

        #region Properties

        public CurveLoopType CurveLookType
        {
            get
            {
                return curveLookType;
            }
        }

        #endregion Properties

        #region Constructors & Core

        public Curve2D(CurveLoopType curveLoopType)
        {
            curveLookType = curveLoopType;

            xCurve = new Curve1D(curveLoopType);
            yCurve = new Curve1D(curveLoopType);
        }

        public void Add(Vector2 value, float time)
        {
            xCurve.Add(value.X, time);
            yCurve.Add(value.Y, time);
        }

        public void Clear()
        {
            xCurve.Clear();
            yCurve.Clear();
        }

        public Vector2 Evaluate(float timeInSecs, int decimalPrecision)
        {
            return new Vector2(xCurve.Evaluate(timeInSecs, decimalPrecision), yCurve.Evaluate(timeInSecs, decimalPrecision));
        }

        #endregion Constructors & Core

        //Add Equals, Clone, ToString, GetHashCode...
    }
}