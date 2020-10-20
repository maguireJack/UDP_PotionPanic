using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary
{
    /// <summary>
    /// Encapsulates the effect, texture, color (diffuse etc ) and alpha fields for any drawn 3D object.
    /// </summary>
    public class EffectParameters : ICloneable
    {
        #region Fields
        //shader reference
        private BasicEffect effect;

        //texture
        private Texture2D texture;

        //transparency
        float alpha;

        //defaults in case the developer forgets to set these values when adding a model object (or child object).
        //setting these values prevents us from seeing only a black surface (i.e. no texture, no color) or no object at all (alpha = 0).
        private Color diffuseColor = Color.White;
        #endregion

        #region Properties
        /// <summary>
        /// Represents the Effect (i.e. shader code) used to render the drawn object
        /// </summary>
        /// <value>
        /// Effect gets/sets the value of the effect field
        /// </value>
        public BasicEffect Effect
        {
            get
            {
                return this.effect;
            }
            set
            {
                this.effect = value;
            }
        }

        /// <summary>
        /// Represents the 2D texture used to render the drawn object
        /// </summary>
        /// <value>
        /// Texture gets/sets the value of the texture field
        /// </value>
        public Texture2D Texture
        {
            get
            {
                return this.texture;
            }
            set
            {
                this.texture = value;
            }
        }

        /// <summary>
        /// Represents the diffuseColor used to blend with the rendered drawn object
        /// </summary>
        /// <value>
        /// DiffuseColor gets/sets the value of the diffuseColor field
        /// </value>
        public Color DiffuseColor
        {
            get
            {
                return this.diffuseColor;
            }
            set
            {
                this.diffuseColor = value;
            }
        }

        /// <summary>
        /// Represents the transparency set on the rendered drawn object
        /// </summary>
        /// <value>
        /// Alpha gets/sets the value of the alpha field
        /// </value>
        public float Alpha
        {
            get
            {
                return this.alpha;
            }
            set
            {
                if (value < 0)
                    this.alpha = 0;
                else if (value > 1)
                    this.alpha = 1;
                else
                    this.alpha = (float)Math.Round(value, 2); //2 decimal places e.g. 0.99
            }
        }

        #endregion

        #region Constructors & Core
        //objects with texture and alpha but no specular or emmissive
        /// <summary>
        /// Constructor for EffectParameters object used by any DrawnActor3D
        /// </summary>
        /// <param name="effect">Basic effect</param>
        /// <param name="texture">2D Texture</param>
        /// <param name="diffusecolor">RGBA diffuse color</param>
        /// <param name="alpha">Floating-point tansparency value</param>
        public EffectParameters(BasicEffect effect, Texture2D texture, Color diffusecolor, float alpha)
        {
            this.Effect = effect;

            if (texture != null)
                Texture = texture;

            DiffuseColor = diffuseColor;

            //use Property to ensure values are inside correct ranges
            this.Alpha = alpha;
        }

        public void Draw(Matrix world, Camera3D camera)
        {
            this.effect.World = world;
            this.effect.View = camera.View;
            this.effect.Projection = camera.Projection;

            //if no texture, then dont crash!
            if(this.texture != null)
                this.effect.Texture = this.texture;

            //to do - diffuse and alpha are not applied
            this.effect.DiffuseColor = this.diffuseColor.ToVector3();
            this.effect.Alpha = this.alpha;
            this.effect.CurrentTechnique.Passes[0].Apply();
        }

        public object Clone()
        {
            //hybrid - shallow and deep
            return new EffectParameters(this.effect, //ref - shallow
                this.texture,  //ref - shallow
                this.diffuseColor, //in-built value types - so deep
                this.alpha); //in-built primitive value types - so deep
        }

        public override bool Equals(object obj)
        {
            return obj is EffectParameters parameters &&
                   EqualityComparer<BasicEffect>.Default.Equals(effect, parameters.effect) &&
                   EqualityComparer<Texture2D>.Default.Equals(texture, parameters.texture) &&
                   alpha == parameters.alpha &&
                   diffuseColor.Equals(parameters.diffuseColor);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(effect, texture, alpha, diffuseColor);
        }
        #endregion
    }
}
