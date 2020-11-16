using GDLibrary.Controllers;
using GDLibrary.Enums;
using GDLibrary.GameComponents;
using JigLibX.Collision;
using JigLibX.Physics;
using Microsoft.Xna.Framework;
using System;

//Physics - Step 2
namespace GDLibrary.Managers
{
    /// <summary>
    /// Enables game physics and CD/cR by integrating forces applied to each collidable object within the scene
    /// </summary>
    public class PhysicsManager : PausableGameComponent
    {
        #region Fields
        private PhysicsSystem physicSystem;
        private PhysicsController physCont;
        private float timeStep = 0;
        #endregion Fields

        #region Properties
        public PhysicsSystem PhysicsSystem
        {
            get
            {
                return physicSystem;
            }
        }
        public PhysicsController PhysicsController
        {
            get
            {
                return physCont;
            }
        }
        #endregion Properties

        //gravity pre-defined
        public PhysicsManager(Game game, StatusType statusType)
            : this(game, statusType, -10 * Vector3.UnitY)
        {
        }

        //user-defined gravity
        public PhysicsManager(Game game, StatusType statusType, Vector3 gravity)
            : base(game, statusType)
        {
            this.physicSystem = new PhysicsSystem();

            //add cd/cr system
            this.physicSystem.CollisionSystem = new CollisionSystemSAP();

            //allows us to define the direction and magnitude of gravity - default is (0, -9.8f, 0)
            //     this.physicSystem.Gravity = gravity;

            //prevents bug where objects would show correct CDCR response when velocity == Vector3.Zero
            this.physicSystem.EnableFreezing = false;

            this.physicSystem.SolverType = PhysicsSystem.Solver.Normal;
            this.physicSystem.CollisionSystem.UseSweepTests = true;

            //affect accuracy and the overhead == time required
            this.physicSystem.NumCollisionIterations = 8;
            this.physicSystem.NumContactIterations = 8;
            this.physicSystem.NumPenetrationRelaxtionTimesteps = 12;

            #region SETTING_COLLISION_ACCURACY
            //affect accuracy of the collision detection
            this.physicSystem.AllowedPenetration = 0.000025f;
            this.physicSystem.CollisionTollerance = 0.00005f;
            #endregion SETTING_COLLISION_ACCURACY

            this.physCont = new PhysicsController();
            this.physicSystem.AddController(physCont);
        }

        protected override void ApplyUpdate(GameTime gameTime)
        {
            timeStep = (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
            //if the time between updates indicates a FPS of close to 60 fps or less then update CD/CR engine
            if (timeStep < 1.0f / 60.0f)
                physicSystem.Integrate(timeStep);
            else
                //else fix at 60 updates per second
                physicSystem.Integrate(1.0f / 60.0f);
        }
    }
}