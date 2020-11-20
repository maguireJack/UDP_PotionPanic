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
        private Texture2D backSky, leftSky, rightSky, frontSky, topSky, grass, crate, wizardTexture, mushroomTexture, redRockTexture, redPotionTexture, spacekey;

        //font used to show debug info
        private SpriteFont debugFont;

        #region Demo

        private PrimitiveObject archetypalTexturedQuad;
        private Curve1D curve1D;

        #endregion

        private PrimitiveObject primitiveObject = null;
        private Model box, wizard, redPotion, level, cauldronModel, redRockModel, blueFlowerModel, greenMushroom;
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

        #region Initialization - Managers, Cameras, Effects, Textures

        protected override void Initialize()
        {
            #region Demo
            DemoCurve();
            DemoViewport();
            #endregion

            //set game title
            Window.Title = "Potion Panic";

            //note that we moved this from LoadContent to allow InitDebug to be called in Initialize
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            InitEventDispatcher();
            //managers
            InitManagers();

            //dictionaries
            InitDictionaries();

            //resources and effects
            InitVertices();
            InitTextures();
            InitModels();
            InitFonts();
            InitEffects();

            //drawn content
            InitDrawnContent();

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

        private void InitEventDispatcher()
        {
            eventDispatcher = new EventDispatcher(this);
            Components.Add(eventDispatcher);

            EventDispatcher.Subscribe(EventCategoryType.Pickup, HandleEvent);
        }

        //TODO Parameters not being passed are textures and modeltype
        private void HandleEvent(EventData eventData)
        {
            if (eventData.EventCategoryType == EventCategoryType.Pickup)
            {
                if (eventData.EventActionType == EventActionType.OnCreate)
                {
                    ArrayList potion_data = eventData.Parameters[0] as ArrayList;
                    //effectparameters
                    EffectParameters effectParameters = new EffectParameters(modelEffect,
                        redPotionTexture,
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

        private void DemoCurve()
        {
            curve1D = new Curve1D(CurveLoopType.Oscillate);
            curve1D.Add(100, 2);
            curve1D.Add(250, 5);
            curve1D.Add(1500, 8);
        }

        private void DemoViewport()
        {
            halfSizeViewport = new Viewport(250, 50, 300, 400);
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
            //camera
            cameraManager = new CameraManager<Camera3D>(this, StatusType.Update);
            Components.Add(cameraManager);

            //keyboard
            keyboardManager = new KeyboardManager(this);
            Components.Add(keyboardManager);

            //gamepad
            gamePadManager = new GamePadManager(this, 1);
            Components.Add(gamePadManager);

            //mouse
            mouseManager = new MouseManager(this, false);
            Components.Add(mouseManager);

            //object
            objectManager = new ObjectManager(this, StatusType.Update, 6, 10);

            //render
            renderManager = new RenderManager(this, StatusType.Drawn, ScreenLayoutType.Single,
                objectManager, cameraManager);

            //physicsManager = new PhysicsManager(this, StatusType.Update, -9.81f * Vector3.UnitY);
            //Components.Add(this.physicsManager);

            Components.Add(renderManager);

            Components.Add(objectManager);
        }

        private void InitDebug()
        {
            //create the debug drawer to draw debug info
            DebugDrawer debugDrawer = new DebugDrawer(this, _spriteBatch, debugFont,
                cameraManager, objectManager);

            //set the debug drawer to be drawn AFTER the object manager to the screen
            debugDrawer.DrawOrder = 2;

            //add the debug drawer to the component list so that it will have its Update/Draw methods called each cycle.
            Components.Add(debugDrawer);
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

            spacekey
                = Content.Load<Texture2D>("Assets/Textures/Props/Crates/spacekey");

            crate
                = Content.Load<Texture2D>("Assets/Textures/Props/Crates/crate1");

            wizardTexture
                = Content.Load<Texture2D>("Assets/Textures/Wizard/wizardTexture");

            mushroomTexture
                = Content.Load<Texture2D>("Assets/Textures/Props/Ingredients/MushRoom1");
            redRockTexture
                = Content.Load<Texture2D>("Assets/Textures/Props/Ingredients/RedCrystal");
            //redPotionTexture
            //    = Content.Load<Texture2D>("Assets/Textures/Props/Potion/potion texture");
        }

        private void InitModels()
        {
            box
                = Content.Load<Model>("Assets/Models/box2");

            wizard
              = Content.Load<Model>("Assets/Models/wizard");

            redPotion
                = Content.Load<Model>("Assets/Models/potion1");

            level
                = Content.Load<Model>("Assets/Models/level");

            cauldronModel
                = Content.Load<Model>("Assets/Models/cauldron");

            redRockModel
                = Content.Load<Model>("Assets/Models/RedCrystal");

            blueFlowerModel
                = Content.Load<Model>("Assets/Models/blueFlower");

            greenMushroom
                = Content.Load<Model>("Assets/Models/Shrooms");


        }

        #endregion Initialization - Managers, Cameras, Effects, Textures

        #region Initialization - Vertices, Archetypes, Helpers, Drawn Content(e.g. Skybox)

        private void InitDrawnContent() //formerly InitPrimitives
        {
            //add archetypes that can be cloned
            InitPrimitiveArchetypes();

            //adds origin helper etc
            InitHelpers();

            //add skybox
            InitSkybox();

            //add grass plane
            InitGround();

            //models
            InitStaticModels();
            InitIngredientGivers();
        }

        private void InitStaticModels()
        {

            ////////Cauldron
            //transform
            Transform3D transform3D = new Transform3D(GameConstants.cauldronPos,
                                new Vector3(0, 0, 0),       //rotation
                                new Vector3(0.07f, 0.07f, 0.07f),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            //effectparameters
            EffectParameters effectParameters = new EffectParameters(modelEffect,
                crate,
                Color.White, 1);

            ModelObject modelObject = new ModelObject("Cauldron", ActorType.Interactable,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, cauldronModel);

            Cauldron cauldron = new Cauldron(modelObject, "Cauldron", GameConstants.defualtInteractionDist);

            objectManager.Add(cauldron);


            /////////Spacebar Helper
            //transform3D = new Transform3D(GameConstants.cauldronPos,
            //                new Vector3(0,0,0),
            //                new Vector3(.14f, .01f, .14f),
            //                -Vector3.UnitZ,
            //                Vector3.UnitY);
            //effectParameters = new EffectParameters(modelEffect,
            //                   crate,
            //                   Color.Yellow,
            //                   1);
            //ModelObject helper = new ModelObject("helper1", ActorType.Decorator, StatusType.Drawn | StatusType.Update, transform3D, effectParameters, grass);
            //objectManager.Add(helper);


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

            modelObject = new ModelObject("Bin", ActorType.Interactable,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, box);

            Bin bin = new Bin(modelObject, "Bin", GameConstants.defualtInteractionDist);

            objectManager.Add(bin);

            ///////Level
            //transform 
            transform3D = new Transform3D(new Vector3(-600, 0, 0),
                                new Vector3(0, 0, 0),       //rotation
                                new Vector3(2, 2, 2),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            //effectparameters
            effectParameters = new EffectParameters(modelEffect,
                null,
                Color.White, 1);

            //model object
            ModelObject levelObject = new ModelObject("level", ActorType.Decorator,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, level);

            objectManager.Add(levelObject);
        }

        private void InitIngredientGivers()
        {
            ///////////////////////////////////Red Rock Giver\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            //transform 
            Transform3D transform3D = new Transform3D(new Vector3(0, 0, -100),
                                new Vector3(0, 0, 0),       //rotation
                                new Vector3(50, 50, 50),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            //effectparameters
            EffectParameters effectParameters = new EffectParameters(modelEffect,
                redRockTexture,
                Color.Red, 1);

            //model object
            ModelObject modelObject = new ModelObject("RedRock", ActorType.Interactable,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, redRockModel);

            //Red rock pickup
            HandHeldPickup pickup = new HandHeldPickup(modelObject, PickupType.Ingredient, "Red Rock",
                GameConstants.defualtInteractionDist, GameConstants.potionRedPos, GameConstants.redSolid);

            ////////////////Giver creation
            /////transform 
            transform3D = new Transform3D(new Vector3(0, 0, -100),
                                new Vector3(0, 0, 0),       //rotation
                                new Vector3(10, 10, 10),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            effectParameters = new EffectParameters(modelEffect,
                crate,
                Color.White, 1);

            modelObject = new ModelObject("RedRockGiver", ActorType.Interactable,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, box);

            IngredientGiver ingredientGiver = new IngredientGiver(modelObject, "Red Rock Giver",
                GameConstants.defualtInteractionDist, pickup);
            objectManager.Add(ingredientGiver);


            ///////////////////////////////////Blue Flower Giver\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            //transform 
            transform3D = new Transform3D(new Vector3(100, 0, -100),
                                new Vector3(0, 90, 0),       //rotation
                                new Vector3(3, 3, 3),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            //effectparameters
            effectParameters = new EffectParameters(modelEffect,
                leftSky,
                Color.White, 1);

            //model object
            modelObject = new ModelObject("BlueFlower", ActorType.Interactable,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, blueFlowerModel);

            //Blue flower pickup
            pickup = new HandHeldPickup(modelObject, PickupType.Ingredient, "Blue Flower",
                GameConstants.defualtInteractionDist,GameConstants.potionRedPos, GameConstants.blueSolid);

            ////////////////Giver Creation
            //transform 
            transform3D = new Transform3D(new Vector3(100, 0, -100),
                                new Vector3(0, 0, 0),       //rotation
                                new Vector3(10, 10, 10),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            effectParameters = new EffectParameters(modelEffect,
                crate,
                Color.White, 1);

            modelObject = new ModelObject("BlueFlowerGiver", ActorType.Interactable,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, box);

            ingredientGiver = new IngredientGiver(modelObject, "Blue Flower Giver",
                GameConstants.defualtInteractionDist, pickup);
            objectManager.Add(ingredientGiver);


            ///////////////////////////////////Green Herb Giver\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            //transform 
            transform3D = new Transform3D(new Vector3(-100, 0, -100),
                                new Vector3(0, 90, 0),       //rotation
                                new Vector3(3, 3, 3),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            //effectparameters
            effectParameters = new EffectParameters(modelEffect,
                mushroomTexture,
                Color.White, 1);

            //model object
            modelObject = new ModelObject("GreenMushroom", ActorType.Interactable,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, greenMushroom);

            pickup = new HandHeldPickup(modelObject, PickupType.Ingredient, "Green Mushroom",
                GameConstants.defualtInteractionDist, GameConstants.potionRedPos, GameConstants.greenSolid);

            ////////////////Giver creation
            //transform 
            transform3D = new Transform3D(new Vector3(-100, 0, -100),
                                new Vector3(0, 0, 0),       //rotation
                                new Vector3(10, 10, 10),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            effectParameters = new EffectParameters(modelEffect,
                crate,
                Color.White, 1);

            modelObject = new ModelObject("GreenHerbGiver", ActorType.Interactable,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, box);

            ingredientGiver = new IngredientGiver(modelObject, "Green Herb Giver",
                GameConstants.defualtInteractionDist, pickup);
            objectManager.Add(ingredientGiver);
        }

        private void InitPlayer()
        {
            //transform
            Transform3D transform3D = new Transform3D(new Vector3(0, 0, 0),
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

            Player player = new Player(objectManager, keyboardManager, gamePadManager, playerObject, controller, interactHelper);
            objectManager.Add(player);

            persistantData = new PersistantData();
        }

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
                vertices, PrimitiveType.TriangleStrip, 2);

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
            PrimitiveType primitiveType;
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

        private void InitGround()
        {
            //grass
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            primitiveObject.ID = "grass";
            primitiveObject.EffectParameters.Texture = grass;
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            primitiveObject.Transform3D.RotationInDegrees = new Vector3(90, 90, 0);
            objectManager.Add(primitiveObject);
        }

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