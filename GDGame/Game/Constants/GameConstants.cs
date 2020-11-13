using GDGame.Game.Enums;
using GDGame.Game.Objects;
using GDLibrary.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections;
using System.Collections.Generic;

namespace GDGame.Game.Constants
{
    public class GameConstants
    {
        #region Common

        public static readonly Keys[][] MoveKeys = { 
            new Keys[]{ Keys.W, Keys.S, Keys.A, Keys.D },
            new Keys[]{ Keys.Up, Keys.Down, Keys.Left, Keys.Right }
        };

        #endregion Common

        #region First Person Camera

        public static readonly float moveSpeed = 0.1f;
        public static readonly float strafeSpeed = 0.075f;
        public static readonly float rotateSpeed = 0.01f;

        #endregion First Person Camera

        #region Flight Camera

        public static readonly float flightMoveSpeed = 0.8f;
        public static readonly float flightStrafeSpeed = 0.6f;
        public static readonly float flightRotateSpeed = 0.01f;

        #endregion Flight Camera

        #region Security Camera

        private static readonly float angularSpeedMultiplier = 10;
        public static readonly float lowAngularSpeed = 10;
        public static readonly float mediumAngularSpeed = lowAngularSpeed * angularSpeedMultiplier;
        public static readonly float hiAngularSpeed = mediumAngularSpeed * angularSpeedMultiplier;

        #endregion Security Camera

        #region Car

        public static readonly float carMoveSpeed = 0.08f;
        public static readonly float carRotateSpeed = 0.06f;

        #endregion Car

        #region Player

        public static readonly float playerMoveSpeed = 0.1f;
        public static readonly float playerRotateSpeed = 4f;

        public static readonly float playerCamOffsetY = 300;
        public static readonly float playerCamOffsetZ = 300;

        public static readonly Vector3 playerHoldPos = new Vector3(32, 40, 3);
        public static readonly Vector3 potionRedPos = new Vector3(4, 18, -1.5f);

        public static readonly Keys[] playerInteractKeys = { Keys.Space, Keys.RightControl };
        public static readonly Buttons[] playerInteractButtons = { Buttons.LeftTrigger };
        public static readonly float defualtInteractionDist = 50f;

        #endregion

        #region Objects

        public static readonly Vector3 cauldronPos = new Vector3(100, 0, 100);
        public static readonly Vector3 binPos = new Vector3(-100, 0, 100);

        #endregion

        #region Ingredients

        public static readonly Ingredient redSolid = new Ingredient(IngredientType.Red, IngredientState.Solid);
        public static readonly Ingredient redDust = new Ingredient(IngredientType.Red, IngredientState.Dust);
        public static readonly Ingredient redLiquid = new Ingredient(IngredientType.Red, IngredientState.Liquid);

        public static readonly Ingredient blueSolid = new Ingredient(IngredientType.Blue, IngredientState.Solid);
        public static readonly Ingredient blueDust = new Ingredient(IngredientType.Blue, IngredientState.Dust);
        public static readonly Ingredient blueLiquid = new Ingredient(IngredientType.Blue, IngredientState.Liquid);

        public static readonly Ingredient greenSolid = new Ingredient(IngredientType.Green, IngredientState.Solid);
        public static readonly Ingredient greenDust = new Ingredient(IngredientType.Green, IngredientState.Dust);
        public static readonly Ingredient greenLiquid = new Ingredient(IngredientType.Green, IngredientState.Liquid);


        #endregion

        #region Potion Data

        //Name, Points, HandPos, Transform3D
        public static readonly ArrayList potion1_data = new ArrayList { "Potion of Healing", 5, new Vector3(4, 18, -1.5f),
            new Transform3D(new Vector3(cauldronPos.X, cauldronPos.Y + 60, cauldronPos.Z),   //translation
                Vector3.Zero,           //rotation
                new Vector3(2, 2, 2),   //scale
                -Vector3.UnitZ,         //look
                Vector3.UnitY)          //up
        };
        public static readonly ArrayList potion2_data = new ArrayList { "Potion of Something", 6, new Vector3(4, 18, -1.5f),
        new Transform3D(new Vector3(cauldronPos.X, cauldronPos.Y + 60, cauldronPos.Z),   //translation
                Vector3.Zero,           //rotation
                new Vector3(2, 2, 2),   //scale
                -Vector3.UnitZ,         //look
                Vector3.UnitY)          //up
        };

        public static readonly Dictionary<Recipe, ArrayList> potions = new Dictionary<Recipe, ArrayList>();

        public static void InitPotions()
        {
            Recipe recipe = new Recipe();
            recipe.Add(redSolid, 2);
            recipe.Add(blueSolid, 1);
            potions.Add(recipe, potion1_data);

            recipe = new Recipe();
            recipe.Add(redSolid, 1);
            recipe.Add(blueSolid, 2);
            potions.Add(recipe, potion2_data);
        }

        #endregion
    }
}