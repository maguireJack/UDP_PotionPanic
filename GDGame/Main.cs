using GDGame.MyGame.Actors;
using GDGame.MyGame.Constants;
using GDGame.MyGame.Controllers;
using GDGame.MyGame.Enums;
using GDGame.MyGame.Objects;
using GDLibrary.Actors;
using GDLibrary.Controllers;
using GDLibrary.Debug;
using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.Factories;
using GDLibrary.Interfaces;
using GDLibrary.Managers;
using GDLibrary.Parameters;
using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections;
using System.Collections.Generic;

namespace GDGame
{
    public class Main : Game
    {
        #region Fields

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //effects used by primitive objects (wireframe, lit, unlit) and model objects
        private BasicEffect unlitTexturedEffect, unlitWireframeEffect, modelEffect;

        //managers in the game
        private CameraManager<Camera3D> cameraManager;
        private ObjectManager objectManager;
        private KeyboardManager keyboardManager;
        private GamePadManager gamePadManager;
        private MouseManager mouseManager;


        //hashmap (Dictonary in C#) to store useful rails and curves
        private Dictionary<string, Transform3DCurve> transform3DCurveDictionary;
        private Dictionary<string, RailParameters> railDictionary;

        //defines centre point for the mouse i.e. (w/2, h/2)
        private Vector2 screenCentre;

        //size of the skybox and ground plane
        private float worldScale = 3000;

        private VertexPositionColorTexture[] vertices;
        private Texture2D backSky, leftSky, rightSky, frontSky, topSky, grass, crate, spacekey, wizardTexture, greenTableTexture,
            greenHerbTexture, cauldronTexture, floorTexture,
            wallLeftTexture, wallRightTexture, redTableTexture, blueTableTexture, redRockTexture, blueFlowerTexture;

        //font used to show debug info
        private SpriteFont debugFont;

        #region Demo

        private PrimitiveObject archetypalTexturedQuad;
        private Curve1D curve1D;

        #endregion

        private PrimitiveObject primitiveObject = null;
        private Model box, wizard, redPotion, floor, cauldronModel, redRockModel, blueFlowerModel,
            greenHerbModel, greenTableModel, wallLeft, wallRight, redTableModel, blueTableModel;
        private EventDispatcher eventDispatcher;
        private PhysicsManager physicsManager;
        private Viewport halfSizeViewport;
        private RenderManager renderManager;
        private PersistantData persistantData;

        #endregion Fields

        #region Constructors

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        #endregion Constructors

        #region Debug
#if DEBUG

        private void InitDebug()
        {
            InitDebugInfo(false);
            InitializeDebugCollisionSkinInfo(true);
        }

        private void InitDebugInfo(bool bEnable)
        {
            if (bEnable)
            {
                //create the debug drawer to draw debug info
                DebugDrawer debugInfoDrawer = new DebugDrawer(this, _spriteBatch, debugFont,
                    cameraManager, objectManager);

                //set the debug drawer to be drawn AFTER the object manager to the screen
                debugInfoDrawer.DrawOrder = 2;

                //add the debug drawer to the component list so that it will have its Update/Draw methods called each cycle.
                Components.Add(debugInfoDrawer);
            }
        }

        private void InitializeDebugCollisionSkinInfo(bool bEnable)
        {
            if (bEnable)
            {
                //show the collision skins
                PhysicsDebugDrawer physicsDebugDrawer = new PhysicsDebugDrawer(this, StatusType.Update | StatusType.Drawn,
                    cameraManager, objectManager);

                //set the debug drawer to be drawn AFTER the object manager to the screen
                physicsDebugDrawer.DrawOrder = 3;

                Components.Add(physicsDebugDrawer);

                //ObjectManager -> Debug -> UIManager -> MenuManager
            }
        }

#endif
        #endregion Debug

        #region Initialization - Managers, Cameras, Effects, Textures

        protected override void Initialize()
        {
            //set game title
            Window.Title = "Potion Panic";

            //note that we moved this from LoadContent to allow InitDebug to be called in Initialize
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //create event dispatcher
            InitEventDispatcher();

            //managers
            InitManagers();

            //dictionaries
            InitDictionaries();

            //resources and effects
            InitVertices();
            InitTextures();
            InitFonts();
            InitEffects();
            InitModels();

            //drawn content
            InitDrawnContent();

            //drawn collidable content
            InitCollidableDrawnContent();

            //curves and rails used by cameras
            InitCurves();
            InitRails();

            //cameras - notice we moved the camera creation BELOW where we created the drawn content - see DriveController
            InitCameras3D();

            GameConstants.InitPotions();
            InitPlayer();

            //graphic settings - see https://en.wikipedia.org/wiki/Display_resolution#/media/File:Vector_Video_Standards8.svg
            InitGraphics(1024, 768);

            //debug info
            InitDebug();

            base.Initialize();
        }

        private void HandleEvent(EventData eventData)
        {
            if (eventData.EventCategoryType == EventCategoryType.Pickup)
            {
                if (eventData.EventActionType == EventActionType.OnCreate)
                {
                    ArrayList potion_data = eventData.Parameters[0] as ArrayList;
                    //effectparameters
                    EffectParameters effectParameters = new EffectParameters(modelEffect,
                        null,
                        Color.White, 1);

                    //model object
                    ModelObject potionObject = new ModelObject((string)potion_data[0] + objectManager.NewID(), ActorType.Interactable,
                        StatusType.Drawn | StatusType.Update, ((Transform3D)potion_data[3]).Clone() as Transform3D,
                        effectParameters, redPotion);

                    HandHeldPickup potion = new HandHeldPickup(potionObject, PickupType.Potion, (string)potion_data[0], 30f, (Vector3)potion_data[2]);
                    objectManager.Add(potion);
                }
            }
        }

        private void InitEventDispatcher()
        {
            eventDispatcher = new EventDispatcher(this);
            Components.Add(eventDispatcher);
        }

        private void InitCurves()
        {
            //create the camera curve to be applied to the track controller
            Transform3DCurve curveA = new Transform3DCurve(CurveLoopType.Oscillate); //experiment with other CurveLoopTypes
            curveA.Add(new Vector3(-500, 1000, 200), -Vector3.UnitY, Vector3.UnitY, 0); //start
            curveA.Add(new Vector3(-500, 250, 400), -Vector3.UnitY, Vector3.UnitY, 1000); //start position
            curveA.Add(new Vector3(-500, 100, 600), -Vector3.UnitZ, Vector3.UnitY, 3000); //start position
            curveA.Add(new Vector3(-500, 50, 800), -Vector3.UnitZ, Vector3.UnitY, 4000); //start position
            curveA.Add(new Vector3(-500, 50, 1000), -Vector3.UnitZ, Vector3.UnitY, 6000); //start position
            //curveA.Add(new Vector3(-500, 50, 2500), -Vector3.UnitZ, Vector3.UnitY, 6000); //start position

            //add to the dictionary
            transform3DCurveDictionary.Add("headshake1", curveA);
        }

        private void InitRails()
        {
            //create the track to be applied to the non-collidable track camera 1
            railDictionary.Add("rail1", new RailParameters("rail1 - parallel to z-axis", new Vector3(20, 10, 50), new Vector3(20, 10, -50)));
        }

        private void InitDictionaries()
        {
            //curves - notice we use a basic Dictionary and not a ContentDictionary since curves and rails are NOT media content
            transform3DCurveDictionary = new Dictionary<string, Transform3DCurve>();

            //rails - store rails used by cameras
            railDictionary = new Dictionary<string, RailParameters>();
        }

        private void InitManagers()
        {
            //physics and CD-CR (moved to top because MouseManager is dependent)
            physicsManager = new PhysicsManager(this, StatusType.Update, -9.81f * Vector3.UnitY);
            Components.Add(physicsManager);

            //camera
            cameraManager = new CameraManager<Camera3D>(this, StatusType.Update);
            Components.Add(cameraManager);

            //keyboard
            keyboardManager = new KeyboardManager(this);
            Components.Add(keyboardManager);

            //mouse
            mouseManager = new MouseManager(this, true, physicsManager);
            Components.Add(mouseManager);

            //gamepad
            gamePadManager = new GamePadManager(this, 1);
            Components.Add(gamePadManager);

            //object
            objectManager = new ObjectManager(this, StatusType.Update, 6, 10);
            Components.Add(objectManager);

            //render
            renderManager = new RenderManager(this, StatusType.Drawn, ScreenLayoutType.Single,
                objectManager, cameraManager);
            Components.Add(renderManager);
        }

        private void InitFonts()
        {
            debugFont = Content.Load<SpriteFont>("Assets/Fonts/debug");
        }

        private void InitCameras3D()
        {
            Transform3D transform3D = null;
            Camera3D camera3D = null;
            Viewport viewPort = new Viewport(0, 0, 1024, 768);

            #region Camera -  Third Person Player Camera

            transform3D = new Transform3D(new Vector3(
                0,
                GameConstants.playerCamOffsetY,
                GameConstants.playerCamOffsetZ),
                new Vector3(0, -1, -1), Vector3.UnitY);

            camera3D = new Camera3D("3rd person player",
                ActorType.Camera3D, StatusType.Update, transform3D,
                ProjectionParameters.StandardDeepSixteenTen, viewPort);
            cameraManager.Add(camera3D);

            #endregion

            #region Camera - First Person

            //transform3D = new Transform3D(new Vector3(10, 10, 20),
            //    new Vector3(0, 0, -1), Vector3.UnitY);

            //camera3D = new Camera3D("1st person",
            //    ActorType.Camera3D, StatusType.Update, transform3D,
            //    ProjectionParameters.StandardDeepSixteenTen, new Viewport(5, 5, 502, 374));

            ////attach a controller
            //camera3D.ControllerList.Add(new FirstPersonController(
            //    "1st person controller A", ControllerType.FirstPerson,
            //    keyboardManager, mouseManager,
            //    GameConstants.moveSpeed, GameConstants.strafeSpeed, GameConstants.rotateSpeed));
            //cameraManager.Add(camera3D);

            #endregion Camera - First Person

            #region Camera - Flight

            //transform3D = new Transform3D(new Vector3(0, 10, 10),
            //            new Vector3(0, 0, -1),
            //            Vector3.UnitY);

            //camera3D = new Camera3D("flight person",
            //    ActorType.Camera3D, StatusType.Update, transform3D,
            //    ProjectionParameters.StandardDeepSixteenTen, new Viewport(0, 384, 512, 384));

            ////define move parameters
            //MoveParameters moveParameters = new MoveParameters(keyboardManager,
            //    mouseManager, GameConstants.flightMoveSpeed, GameConstants.flightStrafeSpeed,
            //    GameConstants.flightRotateSpeed,
            //    GameConstants.MoveKeys[0]);

            //attach a controller
            //camera3D.ControllerList.Add(new FlightCameraController("flight controller",
            //                            ControllerType.FlightCamera, moveParameters));
            //cameraManager.Add(camera3D);

            #endregion Camera - Flight

            #region Camera - Security

            //transform3D = new Transform3D(new Vector3(10, 10, 50),
            //            new Vector3(0, 0, -1),
            //            Vector3.UnitY);

            //camera3D = new Camera3D("security",
            //    ActorType.Camera3D, StatusType.Update, transform3D,
            //    ProjectionParameters.StandardDeepSixteenTen, new Viewport(512, 384, 512, 384));

            //camera3D.ControllerList.Add(new PanController(
            //    "pan controller", ControllerType.Pan,
            //    new Vector3(1, 1, 0), new TrigonometricParameters(30, GameConstants.mediumAngularSpeed, 0)));
            //cameraManager.Add(camera3D);

            #endregion Camera - Security

            #region Camera - Curve3D

            //notice that it doesnt matter what translation, look, and up are since curve will set these
            transform3D = new Transform3D(Vector3.Zero, Vector3.Zero, Vector3.Zero);

            camera3D = new Camera3D("curve camera - main arena",
              ActorType.Camera3D, StatusType.Update, transform3D,
              ProjectionParameters.StandardDeepSixteenTen, viewPort);

            camera3D.ControllerList.Add(new Curve3DController("main arena - fly through - 1",
                ControllerType.Curve,
                        transform3DCurveDictionary["headshake1"])); //use the curve dictionary to retrieve a transform3DCurve by id

            cameraManager.Add(camera3D);

            #endregion Camera - Curve3D

            cameraManager.ActiveCameraIndex = 0; //0, 1, 2, 3
        }

        private void InitEffects()
        {
            //to do...
            unlitTexturedEffect = new BasicEffect(_graphics.GraphicsDevice);
            unlitTexturedEffect.VertexColorEnabled = true; //otherwise we wont see RGB
            unlitTexturedEffect.TextureEnabled = true;

            //wireframe primitives with no lighting and no texture
            unlitWireframeEffect = new BasicEffect(_graphics.GraphicsDevice);
            unlitWireframeEffect.VertexColorEnabled = true;

            //model effect
            //add a ModelObject
            modelEffect = new BasicEffect(_graphics.GraphicsDevice);
            modelEffect.TextureEnabled = true;
            modelEffect.LightingEnabled = true;
            modelEffect.PreferPerPixelLighting = true;
            //   this.modelEffect.SpecularPower = 512;
            //  this.modelEffect.SpecularColor = Color.Red.ToVector3();
            modelEffect.EnableDefaultLighting();
        }

        private void InitTextures()
        {
            //step 1 - texture
            backSky
                = Content.Load<Texture2D>("Assets/Textures/Skybox/back");
            leftSky
               = Content.Load<Texture2D>("Assets/Textures/Skybox/left");
            rightSky
              = Content.Load<Texture2D>("Assets/Textures/Skybox/right");
            frontSky
              = Content.Load<Texture2D>("Assets/Textures/Skybox/front");
            topSky
              = Content.Load<Texture2D>("Assets/Textures/Skybox/sky");

            grass
              = Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1");

            crate
                = Content.Load<Texture2D>("Assets/Textures/Props/Crates/crate1");

            wizardTexture
                = Content.Load<Texture2D>("Assets/Textures/Wizard/wizardTexture");

            greenTableTexture
               = Content.Load<Texture2D>("Assets/Textures/Props/ingredientTables/greenTableTexture");

            greenHerbTexture
                = Content.Load<Texture2D>("Assets/Textures/Props/Ingredients/greenHerbTexture");


            cauldronTexture
               = Content.Load<Texture2D>("Assets/Textures/props/Cauldron/cauldronTexture");

            floorTexture
               = Content.Load<Texture2D>("Assets/Textures/Level/floorTexture");


            wallLeftTexture
               = Content.Load<Texture2D>("Assets/Textures/Level/wallLeftTexture");

            wallRightTexture
               = Content.Load<Texture2D>("Assets/Textures/Level/wallRightTexture");

            redTableTexture
               = Content.Load<Texture2D>("Assets/Textures/Props/ingredientTables/redTableTexture");

            blueTableTexture
               = Content.Load<Texture2D>("Assets/Textures/Props/ingredientTables/blueTableTexture");

            redRockTexture
                = Content.Load<Texture2D>("Assets/Textures/Props/Ingredients/redRockTexture");

            blueFlowerTexture
                = Content.Load<Texture2D>("Assets/Textures/Props/Ingredients/blueFlowerTexture");

        }

        private void InitModels()
        {
            box
                = Content.Load<Model>("Assets/Models/Primitives/box2");

            wizard
              = Content.Load<Model>("Assets/Models/wizard");

            redPotion
                = Content.Load<Model>("Assets/Models/Interactables/potion1");

            floor
                = Content.Load<Model>("Assets/Models/Level/floor");

            cauldronModel
                = Content.Load<Model>("Assets/Models/Interactables/cauldron");

            blueFlowerModel
                = Content.Load<Model>("Assets/Models/Ingredients/blueFlower");

            redRockModel
                = Content.Load<Model>("Assets/Models/Ingredients/redRock");

            greenHerbModel
                = Content.Load<Model>("Assets/Models/Ingredients/greenHerb");

            greenTableModel
                = Content.Load<Model>("Assets/Models/Interactables/Tables/greenTable");

            wallLeft
                = Content.Load<Model>("Assets/Models/Level/wallLeft");

            wallRight
                = Content.Load<Model>("Assets/Models/Level/wallRight");

            redTableModel
                = Content.Load<Model>("Assets/Models/Interactables/Tables/redTable");

            blueTableModel
                = Content.Load<Model>("Assets/Models/Interactables/Tables/blueTable");

        }

        #endregion Initialization - Managers, Cameras, Effects, Textures

        #region Initialization - Vertices, Archetypes, Helpers, Drawn Content(e.g. Skybox)

        private void InitCollidableDrawnContent()
        {

            InitStaticCollidableGround();

            InitStaticCollidableLevel();

            InitStaticCollidableObjects();
        }

        private void InitStaticCollidableGround()
        {
            CollidableObject collidableObject = null;
            Transform3D transform3D = null;
            EffectParameters effectParameters = null;
            Model model = null;

            model = box;

            effectParameters = new EffectParameters(modelEffect,
                  grass,
                  Color.White, 1);

            transform3D = new Transform3D(Vector3.Zero, Vector3.Zero, new Vector3(worldScale, 0.001f, worldScale), -Vector3.UnitZ, Vector3.UnitY);

            collidableObject = new CollidableObject("ground", ActorType.CollidableGround,
                StatusType.Update | StatusType.Drawn,
                transform3D, effectParameters, model);

            //focus on CDCR specific methods and parameters - plane, sphere, box, capsule
            collidableObject.AddPrimitive(new JigLibX.Geometry.Plane(transform3D.Up, transform3D.Translation),
                new MaterialProperties(0.8f, 0.8f, 0.7f));

            collidableObject.Enable(true, 1); //change to false, see what happens.

            objectManager.Add(collidableObject);
        }

        private void InitStaticCollidableLevel()
        {
            ///////Floor
            //transform 
            Transform3D transform3D = new Transform3D(new Vector3(0, 0, 0),
                                new Vector3(0, 0, 0),       //rotation
                                new Vector3(1, 1, 1),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            //effectparameters
            EffectParameters effectParameters = new EffectParameters(modelEffect,
                floorTexture,
                Color.White, 1);

            //model object
            CollidableObject collidableObject = new CollidableObject("floor", ActorType.CollidableDecorator,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, floor);

            collidableObject.AddPrimitive(new Box(new Vector3(100, 0, 100), Matrix.Identity, new Vector3(800, 53, 800)),
                new MaterialProperties(0.2f, 0.8f, 0.7f));

            collidableObject.Enable(true, 1);

            objectManager.Add(collidableObject);

            ///////wallLeft
            //transform 
            transform3D = new Transform3D(new Vector3(0, 0, 0),
                                    new Vector3(0, 0, 0),       //rotation
                                    new Vector3(1, 1, 1),        //scale
                                        -Vector3.UnitZ,         //look
                                        Vector3.UnitY);         //up

            //effectparameters
            effectParameters = new EffectParameters(modelEffect,
                wallLeftTexture,
                Color.White, 1);

            //model object
            collidableObject = new CollidableObject("wallLeft", ActorType.CollidableDecorator,
               StatusType.Drawn | StatusType.Update, transform3D,
               effectParameters, wallLeft);

            //collidableObject.AddPrimitive(new Box(new Vector3(460, -229, 249), Matrix.Identity, new Vector3(80, 430, 1030)),
            collidableObject.AddPrimitive(new JigLibX.Geometry.Plane(transform3D.Up, transform3D.Translation),
                new MaterialProperties(0.2f, 0.8f, 0.7f));

            //collidableObject.Enable(true, 1);

            objectManager.Add(collidableObject);

            ///////wallRight
            //transform 
            transform3D = new Transform3D(new Vector3(0, 0, 0),
                                    new Vector3(0, 0, 0),       //rotation
                                    new Vector3(1, 1, 1),        //scale
                                        -Vector3.UnitZ,         //look
                                        Vector3.UnitY);         //up

            //effectparameters
            effectParameters = new EffectParameters(modelEffect,
                wallRightTexture,
                Color.White, 1);

            //model object
            collidableObject = new CollidableObject("wallRight", ActorType.CollidableDecorator,
               StatusType.Drawn | StatusType.Update, transform3D,
               effectParameters, wallRight);

            collidableObject.AddPrimitive(new Box(transform3D.Translation, Matrix.Identity, transform3D.Scale * 2),
                new MaterialProperties(0.2f, 0.8f, 0.7f));

            collidableObject.Enable(true, 1);

            objectManager.Add(collidableObject);
        }

        private void InitStaticCollidableObjects()
        {
            ////////Cauldron
            //transform
            Transform3D transform3D = new Transform3D(GameConstants.cauldronPos,
                                new Vector3(0, 0, 0),       //rotation
                                new Vector3(0.8f, 0.8f, 0.8f),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            //effectparameters
            EffectParameters effectParameters = new EffectParameters(modelEffect,
                cauldronTexture,
                Color.White, 1);

            CollidableObject collidableObject = new CollidableObject("Cauldron", ActorType.Interactable,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, cauldronModel);

            Cauldron cauldron = new Cauldron(collidableObject, "Cauldron", GameConstants.defualtInteractionDist);
            cauldron.AddPrimitive(new Sphere(GameConstants.cauldronPos + new Vector3(100, -50, 100), 50), new MaterialProperties(0.2f, 0.8f, 0.7f));
            cauldron.Enable(true, 1);

            objectManager.Add(cauldron);


            ////////Bin
            //transform
            transform3D = new Transform3D(GameConstants.binPos,
                                new Vector3(0, 0, 0),       //rotation
                                new Vector3(10, 10, 10),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            //effectparameters
            effectParameters = new EffectParameters(modelEffect,
                crate,
                Color.White, 1);

            collidableObject = new CollidableObject("Bin", ActorType.Interactable,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, box);

            Bin bin = new Bin(collidableObject, "Bin", GameConstants.defualtInteractionDist);
            bin.AddPrimitive(new Box(transform3D.Translation + new Vector3(100, -50, 100), Matrix.Identity, transform3D.Scale * 2),
                new MaterialProperties(0.2f, 0.8f, 0.7f));
            bin.Enable(true, 1);

            objectManager.Add(bin);
        }

        private void InitDrawnContent() //formerly InitPrimitives
        {
            //add archetypes that can be cloned
            InitPrimitiveArchetypes();

            //adds origin helper etc
            InitHelpers();

            //add skybox
            InitSkybox();

            //models
            InitStaticModels();
            InitIngredientGivers();
        }

        #region Static Models

        private void InitStaticModels()
        {

        }

        private void InitIngredientGivers()
        {
            ///////////////////////////////////Red Rock Giver\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            //transform 
            Transform3D transform3D = new Transform3D(new Vector3(-350, 10, -250),
                                new Vector3(0, 0, 0),       //rotation
                                new Vector3(0.5f, 0.5f, 0.5f),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            //effectparameters
            EffectParameters effectParameters = new EffectParameters(modelEffect,
                redRockTexture,
                Color.Red, 1);

            //model object
            CollidableObject collidableObject = new CollidableObject("RedRock", ActorType.Interactable,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, redRockModel);

            //Red rock pickup
            HandHeldPickup pickup = new HandHeldPickup(collidableObject, PickupType.Ingredient, "Red Rock",
                GameConstants.defualtInteractionDist, GameConstants.potionRedPos, GameConstants.redSolid);

            pickup.AddPrimitive(new Box(transform3D.Translation, Matrix.Identity, transform3D.Scale),
                new MaterialProperties(0.2f, 0.8f, 0.7f));

            ////////////////Giver creation
            /////transform 
            transform3D = new Transform3D(new Vector3(-350, 10, -250),
                                new Vector3(0, 0, 0),       //rotation
                                new Vector3(0.7f, 0.7f, 0.7f),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            effectParameters = new EffectParameters(modelEffect,
                redTableTexture,
                Color.White, 1);

            collidableObject = new CollidableObject("RedRockGiver", ActorType.Interactable,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, redTableModel);

            

            IngredientGiver ingredientGiver = new IngredientGiver(collidableObject, "Red Rock Giver",
                GameConstants.defualtInteractionDist, pickup);

            ingredientGiver.AddPrimitive(new Box(transform3D.Translation + new Vector3(-50, 0, -50), Matrix.Identity, new Vector3(101, 83, 125)),
                new MaterialProperties(0.2f, 0.8f, 0.7f));

            

            objectManager.Add(ingredientGiver);


            ///////////////////////////////////Blue Flower Giver\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            //transform 
            transform3D = new Transform3D(new Vector3(-200, 10, -370),
                                new Vector3(0, 90, 0),       //rotation
                                new Vector3(1, 1, 1),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            //effectparameters
            effectParameters = new EffectParameters(modelEffect,
                blueFlowerTexture,
                Color.White, 1);

            //model object
            collidableObject = new CollidableObject("BlueFlower", ActorType.Interactable,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, blueFlowerModel);

            //Blue flower pickup
            pickup = new HandHeldPickup(collidableObject, PickupType.Ingredient, "Blue Flower",
                GameConstants.defualtInteractionDist, GameConstants.potionRedPos, GameConstants.blueSolid);

            pickup.AddPrimitive(new Box(transform3D.Translation, Matrix.Identity, transform3D.Scale),
                new MaterialProperties(0.2f, 0.8f, 0.7f));

            ////////////////Giver Creation
            //transform 
            transform3D = new Transform3D(new Vector3(-200, 10, -370),
                                new Vector3(0, 0, 0),       //rotation
                                new Vector3(0.7f, 0.7f, 0.7f),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            effectParameters = new EffectParameters(modelEffect,
                blueTableTexture,
                Color.White, 1);

            collidableObject = new CollidableObject("BlueFlowerGiver", ActorType.Interactable,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, blueTableModel);

            ingredientGiver = new IngredientGiver(collidableObject, "Blue Flower Giver",
                GameConstants.defualtInteractionDist, pickup);
            objectManager.Add(ingredientGiver);

            ingredientGiver.AddPrimitive(new Box(transform3D.Translation + new Vector3(-50, 0, 0), Matrix.Identity, new Vector3(125, 83, 101)),
                new MaterialProperties(0.2f, 0.8f, 0.7f));
            


            ///////////////////////////////////Green Herb Giver\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            //transform 
            transform3D = new Transform3D(new Vector3(100, 30, -350),
                                new Vector3(0, 90, 0),       //rotation
                                new Vector3(1, 1, 1),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            //effectparameters
            effectParameters = new EffectParameters(modelEffect,
                greenHerbTexture,
                Color.White, 1);

            //model object
            collidableObject = new CollidableObject("GreenMushroom", ActorType.Interactable,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, greenHerbModel);

            pickup = new HandHeldPickup(collidableObject, PickupType.Ingredient, "Green Mushroom",
                GameConstants.defualtInteractionDist, GameConstants.potionRedPos, GameConstants.greenSolid);

            pickup.AddPrimitive(new Box(transform3D.Translation, Matrix.Identity, transform3D.Scale),
                new MaterialProperties(0.2f, 0.8f, 0.7f));

            ////////////////Giver creation
            //transform 
            transform3D = new Transform3D(new Vector3(100, 30, -350),
                                new Vector3(0, 90, 0),       //rotation
                                new Vector3(0.5f, 0.5f, 0.5f),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            effectParameters = new EffectParameters(modelEffect,
                greenTableTexture,
                Color.White, 1);

            collidableObject = new CollidableObject("GreenHerbGiver", ActorType.Interactable,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, greenTableModel);

            ingredientGiver = new IngredientGiver(collidableObject, "Green Herb Giver",
                GameConstants.defualtInteractionDist, pickup);

            ingredientGiver.AddPrimitive(new Box(transform3D.Translation + new Vector3(-70, 0, 0), Matrix.Identity, new Vector3(125, 83, 101)),
                new MaterialProperties(0.2f, 0.8f, 0.7f));
            

            objectManager.Add(ingredientGiver);
        }

        #endregion

        private void InitPlayer()
        {
            //transform
            Transform3D transform3D = new Transform3D(new Vector3(0, 100, 0),
                                new Vector3(0, 0, 0),       //rotation
                                new Vector3(1, 1, 1),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            //effectparameters
            EffectParameters effectParameters = new EffectParameters(modelEffect,
                wizardTexture,
                Color.White, 1);

            //model object
            ModelObject playerObject = new ModelObject("Wizard", ActorType.Player,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, wizard);
            playerObject.ControllerList.Add(new DriveController("player Controler", ControllerType.FirstPerson,
                this.keyboardManager, GameConstants.playerMoveSpeed, GameConstants.playerRotateSpeed));

            ThirdPersonPlayerController controller = new ThirdPersonPlayerController(
                "3rd person player controller", ControllerType.ThirdPerson,
                keyboardManager, mouseManager, cameraManager[0],
                GameConstants.playerMoveSpeed,
                GameConstants.playerRotateSpeed,
                GameConstants.MoveKeys);

            //interact helper
            PrimitiveObject interactHelper = archetypalTexturedQuad.Clone() as PrimitiveObject;
            interactHelper.ID = "spacebar helper";
            interactHelper.EffectParameters.Texture = spacekey;
            interactHelper.Transform3D.Scale = new Vector3(70, 35, 0);
            interactHelper.StatusType = StatusType.Off;
            interactHelper.ActorType = ActorType.Decorator;
            interactHelper.Transform3D.RotationInDegrees = new Vector3(90, 180, 0);
            objectManager.Add(interactHelper);

            Player player = new Player(playerObject, 20, 1, 2, 2,
                objectManager, keyboardManager, gamePadManager, controller, interactHelper);

            player.Enable(false, 10);

            objectManager.Add(player);

            persistantData = new PersistantData();
        }

        #region Vertices, helpers and skybox

        private void InitVertices()
        {
            vertices
                = new VertexPositionColorTexture[4];

            float halfLength = 0.5f;
            //TL
            vertices[0] = new VertexPositionColorTexture(
                new Vector3(-halfLength, halfLength, 0),
                new Color(255, 255, 255, 255), new Vector2(0, 0));

            //BL
            vertices[1] = new VertexPositionColorTexture(
                new Vector3(-halfLength, -halfLength, 0),
                Color.White, new Vector2(0, 1));

            //TR
            vertices[2] = new VertexPositionColorTexture(
                new Vector3(halfLength, halfLength, 0),
                Color.White, new Vector2(1, 0));

            //BR
            vertices[3] = new VertexPositionColorTexture(
                new Vector3(halfLength, -halfLength, 0),
                Color.White, new Vector2(1, 1));
        }

        private void InitPrimitiveArchetypes() //formerly InitTexturedQuad
        {
            Transform3D transform3D = new Transform3D(Vector3.Zero, Vector3.Zero,
               Vector3.One, Vector3.UnitZ, Vector3.UnitY);

            EffectParameters effectParameters = new EffectParameters(unlitTexturedEffect,
                grass, /*bug*/ Color.White, 1);

            IVertexData vertexData = new VertexData<VertexPositionColorTexture>(
                vertices, Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip, 2);

            archetypalTexturedQuad = new PrimitiveObject("original texture quad",
                ActorType.Decorator,
                StatusType.Update | StatusType.Drawn,
                transform3D, effectParameters, vertexData);
        }

        //VertexPositionColorTexture - 4 bytes x 3 (x,y,z) + 4 bytes x 3 (r,g,b) + 4bytes x 2 = 26 bytes
        //VertexPositionColor -  4 bytes x 3 (x,y,z) + 4 bytes x 3 (r,g,b) = 24 bytes
        private void InitHelpers()
        {
            //to do...add wireframe origin
            Microsoft.Xna.Framework.Graphics.PrimitiveType primitiveType;
            int primitiveCount;

            //step 1 - vertices
            VertexPositionColor[] vertices = VertexFactory.GetVerticesPositionColorOriginHelper(
                                    out primitiveType, out primitiveCount);

            //step 2 - make vertex data that provides Draw()
            IVertexData vertexData = new VertexData<VertexPositionColor>(vertices,
                                    primitiveType, primitiveCount);

            //step 3 - make the primitive object
            Transform3D transform3D = new Transform3D(new Vector3(0, 20, 0),
                Vector3.Zero, new Vector3(10, 10, 10),
                Vector3.UnitZ, Vector3.UnitY);

            EffectParameters effectParameters = new EffectParameters(unlitWireframeEffect,
                null, Color.White, 1);

            //at this point, we're ready!
            PrimitiveObject primitiveObject = new PrimitiveObject("origin helper",
                ActorType.Helper, StatusType.Drawn, transform3D, effectParameters, vertexData);

            objectManager.Add(primitiveObject);
        }

        private void InitSkybox()
        {
            //back
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            //  primitiveObject.StatusType = StatusType.Off; //Experiment of the effect of StatusType
            primitiveObject.ID = "sky back";
            primitiveObject.EffectParameters.Texture = backSky;
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, 180, 0);
            primitiveObject.Transform3D.Translation = new Vector3(0, 0, -worldScale / 2.0f);
            objectManager.Add(primitiveObject);

            //left
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            primitiveObject.ID = "left back";
            primitiveObject.EffectParameters.Texture = leftSky;
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, 90, 0);
            primitiveObject.Transform3D.Translation = new Vector3(worldScale / 2.0f, 0, 0);
            objectManager.Add(primitiveObject);

            //right
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            primitiveObject.ID = "sky right";
            primitiveObject.EffectParameters.Texture = rightSky;
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 20);
            primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, -90, 0);
            primitiveObject.Transform3D.Translation = new Vector3(-worldScale / 2.0f, 0, 0);
            objectManager.Add(primitiveObject);

            //top
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            primitiveObject.ID = "sky top";
            primitiveObject.EffectParameters.Texture = topSky;
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            primitiveObject.Transform3D.RotationInDegrees = new Vector3(-90, -90, 0);
            primitiveObject.Transform3D.Translation = new Vector3(0, worldScale / 2.0f, 0);
            objectManager.Add(primitiveObject);

            //front
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            primitiveObject.ID = "sky front";
            primitiveObject.EffectParameters.Texture = frontSky;
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            primitiveObject.Transform3D.Translation = new Vector3(0, 0, worldScale / 2.0f);
            objectManager.Add(primitiveObject);
        }

        #endregion

        private void InitGraphics(int width, int height)
        {
            //set resolution
            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;

            //dont forget to apply resolution changes otherwise we wont see the new WxH
            _graphics.ApplyChanges();

            //set screen centre based on resolution
            screenCentre = new Vector2(width / 2, height / 2);

            //set cull mode to show front and back faces - inefficient but we will change later
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            _graphics.GraphicsDevice.RasterizerState = rs;

            //we use a sampler state to set the texture address mode to solve the aliasing problem between skybox planes
            SamplerState samplerState = new SamplerState();
            samplerState.AddressU = TextureAddressMode.Clamp;
            samplerState.AddressV = TextureAddressMode.Clamp;
            _graphics.GraphicsDevice.SamplerStates[0] = samplerState;

            //set blending
            _graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            //set screen centre for use when centering mouse
            screenCentre = new Vector2(width / 2, height / 2);
        }

        protected override void LoadContent()
        {
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        #endregion Initialization - Vertices, Archetypes, Helpers, Drawn Content(e.g. Skybox)

        #region Update & Draw

        protected override void Update(GameTime gameTime)
        {
            #region Demo

            //System.Diagnostics.Debug.WriteLine("t in ms:" + gameTime.TotalGameTime.TotalMilliseconds + " v: " + curve1D.Evaluate(gameTime.TotalGameTime.TotalMilliseconds, 2));

            #endregion

            if (keyboardManager.IsFirstKeyPress(Keys.Escape))
            {
                persistantData.SaveData();
                Exit();
            }

            if (keyboardManager.IsFirstKeyPress(Keys.C))
            {
                cameraManager.CycleActiveCamera();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            _graphics.GraphicsDevice.DepthStencilState = dss;

            base.Draw(gameTime);
        }

        #endregion Update & Draw
    }
}