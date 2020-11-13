using Microsoft.Xna.Framework;

namespace GDLibrary.Core.Parameters.Time
{
    public class Timer
    {
        #region Fields

        private double timerLengthMS;
        private double startTime;
        private double elapsedTime;

        #endregion

        #region Fields

        public double TimerLengthMS
        { 
            get { return timerLengthMS; } 
            set { timerLengthMS = value; }
        }

        #endregion

        #region Constructors

        public Timer(double timerLengthMS)
        {
            this.timerLengthMS = timerLengthMS;
        }

        #endregion

        public void StartTimer(GameTime gameTime)
        {
            startTime = gameTime.TotalGameTime.TotalMilliseconds;
            elapsedTime = 0;
        }

        public bool IsDone(GameTime gameTime)
        {
            elapsedTime = gameTime.TotalGameTime.TotalMilliseconds - startTime;
            return elapsedTime >= timerLengthMS;
        }
    }
}
