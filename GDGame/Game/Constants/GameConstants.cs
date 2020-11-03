using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDGame
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
        public static readonly float playerRotateSpeed = 0.01f;

        public static readonly float playerCamOffsetY = 500;
        public static readonly float playerCamOffsetZ = 500;

        public static readonly Vector3 playerHoldPos = new Vector3(32, 40, 3);
        public static readonly Vector3 potionRedPos = new Vector3(4, 18, -1.5f);

        #endregion
    }
}