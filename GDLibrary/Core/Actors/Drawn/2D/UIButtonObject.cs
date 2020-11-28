using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary.Actors
{
    /// <summary>
    /// Draws a texture and text to the screen to create a button with a user-defined text string and font. Used primarily by the menu manager
    /// </summary>
    public class UIButtonObject : UITextureObject
    {
        //now this depth will always be less (i.e. close to 0 and forward) than the background texture
        private static float TEXT_LAYER_DEPTH_MULTIPLIER = 0.95f;

        #region Fields
        private string text;
        private SpriteFont spriteFont;
        private Color textColor;
        private Vector2 textOrigin, textOffset;
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
                this.textOrigin = this.spriteFont.MeasureString(text) / 2.0f;
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
        public Color TextColor
        {
            get
            {
                return this.textColor;
            }
            set
            {
                this.textColor = value;
            }
        }
        public Vector2 TextOffset
        {
            get
            {
                return this.textOffset;
            }
            set
            {
                this.textOffset = value;
            }
        }
        #endregion Properties

        #region Constructors & Core
        public UIButtonObject(string id, ActorType actorType, StatusType statusType,
        Transform2D transform2D, Color color, float layerDepth, SpriteEffects spriteEffects,
        Texture2D texture, Rectangle sourceRectangle,
         string text, SpriteFont spriteFont, Color textColor, Vector2 textOffset)
         : base(id, actorType, statusType, transform2D, color, layerDepth, spriteEffects, texture, sourceRectangle)
        {
            Text = text;
            SpriteFont = spriteFont;
            TextColor = textColor;
            TextOffset = textOffset;
        }

        //to do...Draw, Equals, GetHashCode, Clone

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //draw the texture
            base.Draw(gameTime, spriteBatch);

            spriteBatch.DrawString(this.spriteFont, this.text,
                this.Transform2D.Translation + this.textOffset,
                this.Color,
                this.Transform2D.RotationInRadians,
                this.Transform2D.Origin, //giving the text its own origin?
                this.Transform2D.Scale,
                this.SpriteEffects,

                this.LayerDepth * TEXT_LAYER_DEPTH_MULTIPLIER); //now this depth will always be less (i.e. close to 0 and forward) than the background texture
        }

        #endregion Constructors & Core
    }
}