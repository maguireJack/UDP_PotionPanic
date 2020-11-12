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

        private static readonly float strafeSpeedMultiplier = 0.75f;
        public static readonly Keys[] KeysOne = { Keys.W, Keys.S, Keys.A, Keys.D };
        public static readonly Keys[] KeysTwo = { Keys.U, Keys.J, Keys.H, Keys.K };

        #endregion Common

        #region First Person Camera

        public static readonly float moveSpeed = 0.1f;
        public static readonly float strafeSpeed = strafeSpeedMultiplier * moveSpeed;
        public static readonly float rotateSpeed = 0.01f;

        #endregion First Person Camera

        #region Flight Camera

        public static readonly float flightMoveSpeed = 0.8f;
        public static readonly float flightStrafeSpeed = strafeSpeedMultiplier * flightMoveSpeed;
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
        public static readonly float playerStrafeSpeed = strafeSpeedMultiplier * playerMoveSpeed;
        public static readonly float playerRotateSpeed = 4f;

        public static readonly float playerCamOffsetY = 300;
        public static readonly float playerCamOffsetZ = 300;

        public static readonly Vector3 playerHoldPos = new Vector3(32, 40, 3);
        public static readonly Vector3 potionRedPos = new Vector3(4, 18, -1.5f);

        public static readonly Keys playerInteractKey = Keys.Space;
        public static readonly float defualtInteractionDist = 50f;

        #endregion

        #region Objects

        public static readonly Vector3 cauldronPos = new Vector3(100, 0, 100);
        public static readonly Vector3 binPos = new Vector3(-100, 0, 100);

        #endregion

        #region Ingredients

        public static readonly string redSolid = "Red Rock";
        public static readonly string redDust = "Red Dust";
        public static readonly string redLiquid = "Red Liquid";

        public static readonly string blueSolid = "Blue Flower";
        public static readonly string blueDust = "Blue Dust";
        public static readonly string blueLiquid = "Blue Liquid";

        public static readonly string greenSolid = "Green Herb";
        public static readonly string greenDust = "Green Dust";
        public static readonly string greenLiquid = "Green Liquid";

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

        #endregion

            #region Potion Recipes

            /*
             * The naming convention for a potion is:
             * potion = ingredientX + 1, ingredientY + 1, ingredientZ + 1
             * potion = ingredientX + 1, ingredientX + 2, ingredientX + 3
             * Notice, if there are multiple of the same ingredients, add an extra number
            */
        public static readonly HashSet<string> potion1 = new HashSet<string>{ redSolid + 1, redSolid + 2, blueSolid + 1};
        public static readonly HashSet<string> potion2 = new HashSet<string> { blueSolid + 1, blueSolid + 2, redSolid + 1 };

        //add potions into here
        public static readonly Dictionary<HashSet<string>, ArrayList> potions = new Dictionary<HashSet<string>, ArrayList>()
        {
            { potion1, potion1_data },
            { potion2, potion2_data }
        };

        #endregion
    }
}