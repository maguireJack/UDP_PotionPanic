using Microsoft.Xna.Framework;

namespace GDLibrary.Core.Parameters.Time
{
    public class Timer
    {
        #region Fields

        private double timerLengthMS;
        private double startTime;
        private double elapsedTime;
        private bool isRunning;

        #endregion

        #region Fields

        public double TimerLengthMS
        { 
            get { return timerLengthMS; } 
            set { timerLengthMS = value; }
        }

        public bool IsRunning
        {
            get { return isRunning; }
        }

        #endregion

        #region Constructors

        public Timer(double timerLengthMS)
        {
            this.timerLengthMS = timerLengthMS;
            isRunning = false;
        }

        #endregion

        public void StartTimer(GameTime gameTime)
        {
            startTime = gameTime.TotalGameTime.TotalMilliseconds;
            elapsedTime = 0;
            isRunning = true;
        }

        public bool IsDone(GameTime gameTime)
        {
            elapsedTime = gameTime.TotalGameTime.TotalMilliseconds - startTime;
            isRunning = false;
            return elapsedTime >= timerLengthMS;
        }
    }
}
