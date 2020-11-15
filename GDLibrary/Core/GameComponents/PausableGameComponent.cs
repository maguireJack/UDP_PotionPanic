using GDLibrary.Enums;
using Microsoft.Xna.Framework;

namespace GDLibrary.GameComponents
{
    /// <summary>
    /// Creates a class based on the GameComponent class that can be paused when the menu is shown.
    /// </summary>
    /// <see cref="GDLibrary.Managers.KeyboardManager"/>
    public class PausableGameComponent : GameComponent
    {
        #region Fields
        private StatusType statusType;
        #endregion

        #region Properties 
        public StatusType StatusType
        {
            get
            {
                return this.statusType;
            }
            set
            {
                this.statusType = value;
            }
        }
        #endregion

        #region Constructors & Core
        public PausableGameComponent(Game game, StatusType statusType)
           : base(game)
        {
            //allows us to start the game component with drawing and/or updating paused
            this.statusType = statusType;
        }

        public override void Update(GameTime gameTime)
        {
            //screen manager needs to listen to input even when paused i.e. hide/show menu - see ScreenManager::HandleInput()
            HandleInput(gameTime);

            if ((this.statusType & StatusType.Update) != 0) //if update flag is set
                ApplyUpdate(gameTime);
            // base.Update(gameTime); //does notthing so comment out
        }


        protected virtual void ApplyUpdate(GameTime gameTime)
        {

        }

        protected virtual void HandleInput(GameTime gameTime)
        {

        }

        protected virtual void HandleMouse(GameTime gameTime)
        {

        }

        protected virtual void HandleKeyboard(GameTime gameTime)
        {

        }

        protected virtual void HandleGamePad(GameTime gameTime)
        {

        } 
        #endregion
    }
}