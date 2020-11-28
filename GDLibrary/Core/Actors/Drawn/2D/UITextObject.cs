using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary.Actors
{
    /// <summary>
    /// Draws text to the screen with a user-defined text string and font. Useful for showing a score, elapsed time, or other game-state related info
    /// </summary>
    public class UITextObject : DrawnActor2D
    {
        #region Fields
        private string text;
        private SpriteFont spriteFont;
        #endregion Fields

        #region Properties
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = (value.Length >= 0) ? value : "Default";
            }
        }
        public SpriteFont SpriteFont
        {
            get
            {
                return this.spriteFont;
            }
            set
            {
                this.spriteFont = value;
            }
        }
        #endregion Properties

        #region Constructors & Core
        public UITextObject(string id, ActorType actorType, StatusType statusType,
          Transform2D transform2D, Color color, float layerDepth, SpriteEffects spriteEffects,
          string text, SpriteFont spriteFont)
           : base(id, actorType, statusType, transform2D, color, layerDepth, spriteEffects)
        {
            SpriteFont = spriteFont;
            Text = text;
        }

        //to do...Draw, Equals, GetHashCode, Clone

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(this.spriteFont, this.text, this.Transform2D.Translation, this.Color,
                this.Transform2D.RotationInRadians, this.Transform2D.Origin, this.Transform2D.Scale,
                this.SpriteEffects, this.LayerDepth);

            //base.Draw(gameTime, spriteBatch);
        }

        #endregion Constructors & Core
    }
}