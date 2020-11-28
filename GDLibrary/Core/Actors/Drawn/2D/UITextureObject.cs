using GDLibrary.Containers;
using GDLibrary.Enums;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GDLibrary.Actors
{
    /// <summary>
    /// Draws a texture to the screen. Useful for creating health/ammo UI icons, decals around UI elements, or menu backgrounds
    /// </summary>
    public class UITextureObject : DrawnActor2D
    {
        #region Fields
        private Texture2D texture;
        private Rectangle sourceRectangle, originalSourceRectangle;

        #endregion Fields

        #region Properties
        public Texture2D Texture { get => texture; set => texture = value; }
        public Rectangle SourceRectangle { get => sourceRectangle; set => sourceRectangle = value; }

        public int SourceRectangleWidth
        {
            get
            {
                return this.sourceRectangle.Width;
            }
            set
            {
                this.sourceRectangle.Width = value;
            }
        }
        public int SourceRectangleHeight
        {
            get
            {
                return this.sourceRectangle.Height;
            }
            set
            {
                this.sourceRectangle.Height = value;
            }
        }
        public Rectangle OriginalSourceRectangle
        {
            get
            {
                return this.originalSourceRectangle;
            }
        }
        #endregion Properties

        #region Constructors & Core
        public UITextureObject(string id, ActorType actorType, StatusType statusType,
           Transform2D transform2D, Color color, float layerDepth, SpriteEffects spriteEffects,
           Texture2D texture, Rectangle sourceRectangle)
           : base(id, actorType, statusType, transform2D, color, layerDepth, spriteEffects)
        {
            this.Texture = texture;
            this.SourceRectangle = sourceRectangle;
            //store the original source rectangle in case we change the source rectangle (i.e. UIProgressController)
            this.originalSourceRectangle = SourceRectangle;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.texture, this.Transform2D.Translation, this.sourceRectangle,
                this.Color, this.Transform2D.RotationInRadians, this.Transform2D.Origin, this.Transform2D.Scale,
                this.SpriteEffects, this.LayerDepth);

            //base.Draw(gameTime);
        }

        public override bool Equals(object obj)
        {
            return obj is UITextureObject ui &&
                   base.Equals(obj) &&
                   EqualityComparer<Texture2D>.Default.Equals(texture, ui.texture) &&
                   sourceRectangle.Equals(ui.sourceRectangle);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(base.GetHashCode());
            hash.Add(texture);
            hash.Add(sourceRectangle);
            return hash.ToHashCode();
        }

        //to do..Clone

        #endregion Constructors & Core
    }
}