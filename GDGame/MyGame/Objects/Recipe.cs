using System;
using System.Collections.Generic;

namespace GDGame.MyGame.Objects
{
    public class Recipe
    {
        #region Fields

        private Dictionary<Ingredient, int> ingredients;

        #endregion

        #region Properties

        public Dictionary<Ingredient, int> Ingredients
        {
            get { return ingredients; }
        }

        #endregion

        #region Constructors

        public Recipe()
        {
            ingredients = new Dictionary<Ingredient, int>();
        }

        public Recipe(Dictionary<Ingredient, int> ingredients)
        {
            this.ingredients = ingredients;
        }

        #endregion

        public void Add(Ingredient ingredient, int count)
        {
            ingredients.Add(ingredient, count);
        }

        public bool ContainsKey(Ingredient ingredient)
        {
            return ingredients.ContainsKey(ingredient);
        }

        public void Clear()
        {
            ingredients.Clear();
        }
        /// <summary>
        /// Checks if the passed in recipe is equal to the current recipe
        /// </summary>
        /// <param name="obj">Recapie</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if(obj is Recipe)
            {
                Recipe other = obj as Recipe;
                //same amount of keys
                if (ingredients.Count == other.ingredients.Count)
                {
                    foreach (Ingredient key in ingredients.Keys)
                    {
                        //do they both contain same key
                        if(!other.ingredients.ContainsKey(key))
                        {
                            return false;
                        }
                        //do the same keys have the same values
                        if(ingredients[key] != other.ingredients[key])
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ingredients);
        }



    }
}
