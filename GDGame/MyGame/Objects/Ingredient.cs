using GDGame.MyGame.Enums;
using System;

namespace GDGame.MyGame.Objects
{
    public class Ingredient : ICloneable
    {
        #region Fields

        private IngredientType ingredientType;
        private IngredientState ingredientState;

        #endregion

        #region Properties

        public IngredientType IngredientType
        {
            get
            {
                return ingredientType;
            }
        }

        public IngredientState IngredientState
        {
            get
            {
                return ingredientState;
            }
        }

        public string Name
        {
            get
            {
                return IngredientType + "_" + IngredientState;
            }
        }

        #endregion

        #region Constructors

        public Ingredient(IngredientType ingredientType, IngredientState ingredientState)
        {
            this.ingredientType = ingredientType;
            this.ingredientState = ingredientState;
        }

        #endregion

        public void Process()
        {
            ingredientState++;
        }

        public override bool Equals(object obj)
        {
            return obj is Ingredient ingredient &&
                   ingredientType == ingredient.ingredientType &&
                   ingredientState == ingredient.ingredientState;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ingredientType, ingredientState);
        }

        public object Clone()
        {
            return new Ingredient(IngredientType, IngredientState);
        }
    }
}
