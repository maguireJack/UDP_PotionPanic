﻿using GDGame.MyGame.Actors;
using GDGame.MyGame.Constants;
using GDGame.MyGame.Controllers;
using GDGame.MyGame.Enums;
using GDGame.MyGame.Objects;
using GDLibrary.Actors;
using GDLibrary.Containers;
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

//remove this be4fore pus=h
using System.Diagnostics;

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
        private EventDispatcher eventDispatcher;
        private PhysicsManager physicsManager;
        private RenderManager renderManager;
        private PersistantData persistantData;
        private UIManager uiManager;
        private MenuManager menuManager;

        //Debug
        DebugDrawer debugInfoDrawer;
        PhysicsDebugDrawer physicsDebugDrawer;

        //store useful game resources (e.g. effects, models, rails and curves)
        private Dictionary<string, BasicEffect> effectDictionary;

        //use ContentDictionary to store assets (i.e. file content) that need the Content.Load() method to be called
        private ContentDictionary<Texture2D> textureDictionary;
        private ContentDictionary<SpriteFont> fontDictionary;
        private ContentDictionary<Model> modelDictionary;

        //hashmap (Dictonary in C#) to store useful rails and curves
        private Dictionary<string, Transform3DCurve> transform3DCurveDictionary;
        private Dictionary<string, RailParameters> railDictionary;

        //defines centre point for the mouse i.e. (w/2, h/2)
        private Vector2 screenCentre;

        //size of the skybox and ground plane
        private float worldScale = 3000;

        private VertexPositionColorTexture[] vertices;

        //font used to show debug info
        private SpriteFont debugFont;

        #region Demo

        private PrimitiveObject archetypalTexturedQuad;
        private Curve1D curve1D;

        #endregion

        private PrimitiveObject primitiveObject = null;
        private Viewport halfSizeViewport;
        private bool isPaused;

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
            InitDebugInfo(true);
            InitializeDebugCollisionSkinInfo(true);
        }

        private void InitDebugInfo(bool bEnable)
        {
            if (bEnable)
            {
                //create the debug drawer to draw debug info
                debugInfoDrawer = new DebugDrawer(this, _spriteBatch,
                    Content.Load<SpriteFont>("Assets/Fonts/debug"),
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
                physicsDebugDrawer = new PhysicsDebugDrawer(this, StatusType.Update | StatusType.Drawn,
                    cameraManager, objectManager);

                //set the debug drawer to be drawn AFTER the object manager to the screen
                physicsDebugDrawer.DrawOrder = 3;

                Components.Add(physicsDebugDrawer);

                //ObjectManager -> Debug -> UIManager -> MenuManager
            }
        }

#endif
        #endregion Debug

        #region Load - Assets

        private void LoadEffects()
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

        private void LoadTextures()
        {
            //sky
            textureDictionary.Load("Assets/Textures/Skybox/sky_back");
            textureDictionary.Load("Assets/Textures/Skybox/sky_left");
            textureDictionary.Load("Assets/Textures/Skybox/sky_right");
            textureDictionary.Load("Assets/Textures/Skybox/sky_front");
            textureDictionary.Load("Assets/Textures/Skybox/sky");

            //floor
            textureDictionary.Load("Assets/Textures/Foliage/Ground/grass1");
            textureDictionary.Load("Assets/Textures/Level/nebula_red");
            textureDictionary.Load("Assets/Textures/Level/floor");
            textureDictionary.Load("Assets/Textures/Level/lava");

            //ui
            textureDictionary.Load("Assets/Textures/UI/helper_space");
            textureDictionary.Load("Assets/Textures/UI/ring");
            textureDictionary.Load("Assets/Textures/UI/ball");

            //menu
            textureDictionary.Load("Assets/Textures/UI/Aidan'sPotion");

            //walls
            textureDictionary.Load("Assets/Textures/Level/wall_left");
            textureDictionary.Load("Assets/Textures/Level/wall_right");

            //interactables
            textureDictionary.Load("Assets/Textures/Props/ingredientTables/lectern");
            textureDictionary.Load("Assets/Textures/props/Cauldron/cauldron");
            textureDictionary.Load("Assets/Textures/props/Chest/chest");

            textureDictionary.Load("Assets/Textures/Props/ingredientTables/table_red");
            textureDictionary.Load("Assets/Textures/Props/ingredientTables/table_blue");
            textureDictionary.Load("Assets/Textures/Props/ingredientTables/table_green");
            textureDictionary.Load("Assets/Textures/Props/ingredientTables/table_grinder");
            textureDictionary.Load("Assets/Textures/Props/ingredientTables/table_liquid");

            //ingredients
            textureDictionary.Load("Assets/Textures/Props/Ingredients/Red_Solid");
            textureDictionary.Load("Assets/Textures/Props/Ingredients/Blue_Solid");
            textureDictionary.Load("Assets/Textures/Props/Ingredients/Green_Solid");
            textureDictionary.Load("Assets/Textures/Props/Ingredients/Red_Dust");
            textureDictionary.Load("Assets/Textures/Props/Ingredients/Blue_Dust");
            textureDictionary.Load("Assets/Textures/Props/Ingredients/Green_Dust");
            textureDictionary.Load("Assets/Textures/Props/Ingredients/Red_Liquid");
            textureDictionary.Load("Assets/Textures/Props/Ingredients/Blue_Liquid");
            textureDictionary.Load("Assets/Textures/Props/Ingredients/Green_Liquid");

            //other
            textureDictionary.Load("Assets/Textures/Props/Crates/crate");
            textureDictionary.Load("Assets/Textures/Wizard/wizard");
            textureDictionary.Load("Assets/Textures/Props/Potion/potion1");
        }

        private void LoadModels()
        {
            //level
            modelDictionary.Load("Assets/Models/Level/wall_left");
            modelDictionary.Load("Assets/Models/Level/wall_right");
            modelDictionary.Load("Assets/Models/Level/floor");
            modelDictionary.Load("Assets/Models/Level/railing");

            //outer level
            modelDictionary.Load("Assets/Models/Level/outerWalls");
            modelDictionary.Load("Assets/Models/Level/outerWallsRight");
            modelDictionary.Load("Assets/Models/Level/lava");

            //interactables
            modelDictionary.Load("Assets/Models/Interactables/cauldron");
            modelDictionary.Load("Assets/Models/Interactables/Tables/lectern");
            modelDictionary.Load("Assets/Models/Interactables/chest");

            modelDictionary.Load("Assets/Models/Interactables/Tables/table_red");
            modelDictionary.Load("Assets/Models/Interactables/Tables/table_blue");
            modelDictionary.Load("Assets/Models/Interactables/Tables/table_green");
            modelDictionary.Load("Assets/Models/Interactables/Tables/table_grinder");
            modelDictionary.Load("Assets/Models/Interactables/Tables/table_liquid");

            //ingredients
            modelDictionary.Load("Assets/Models/Ingredients/Red_Solid");
            modelDictionary.Load("Assets/Models/Ingredients/Blue_Solid");
            modelDictionary.Load("Assets/Models/Ingredients/Green_Solid");
            modelDictionary.Load("Assets/Models/Ingredients/Red_Dust");
            modelDictionary.Load("Assets/Models/Ingredients/Blue_Dust");
            modelDictionary.Load("Assets/Models/Ingredients/Green_Dust");
            modelDictionary.Load("Assets/Models/Ingredients/Red_Liquid");
            modelDictionary.Load("Assets/Models/Ingredients/Blue_Liquid");
            modelDictionary.Load("Assets/Models/Ingredients/Green_Liquid");

            //other
            modelDictionary.Load("Assets/Models/wizard");
            modelDictionary.Load("Assets/Models/Primitives/box2");
            modelDictionary.Load("Assets/Models/Interactables/potion1");
        }

        private void LoadFonts()
        {
            fontDictionary.Load("Assets/Fonts/debug");
            fontDictionary.Load("Assets/Fonts/ui");
        }

        private void LoadVertices()
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

        #endregion

        #region Initialization - Managers, Cameras, Effects, Textures

        protected override void Initialize()
        {
            //set game title
            Window.Title = "Potion Panic";

            screenCentre = GameConstants.screenCentre;
            isPaused = false;

            //note that we moved this from LoadContent to allow InitDebug to be called in Initialize
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //create event dispatcher
            InitEventDispatcher();

            //managers
            InitManagers();

            //dictionaries
            InitDictionaries();

            //load from file or initialize assets, effects and vertices
            GameConstants.LoadData();
            LoadEffects();
            LoadTextures();
            LoadVertices();
            LoadModels();
            LoadFonts();

            //ui
            InitUI(); 
            InitMenu();

            //drawn content
            InitDrawnContent();

            //drawn collidable content
            InitCollidableDrawnContent();

            //curves and rails used by cameras
            InitCurves();
            InitRails();

            //cameras - notice we moved the camera creation BELOW where we created the drawn content - see DriveController
            InitCameras3D();

            InitPlayer();

            //graphic settings - see https://en.wikipedia.org/wiki/Display_resolution#/media/File:Vector_Video_Standards8.svg
            InitGraphics(1440, 1080);

            //debug info
            InitDebug();
            debugInfoDrawer.Visible = false;
            physicsDebugDrawer.Visible = false;

            base.Initialize();
        }

        private void InitGraphics(int width, int height)
        {
            //set resolution
            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;

            //dont forget to apply resolution changes otherwise we wont see the new WxH
            _graphics.ApplyChanges();

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

            //add this code to centre the mouse when the game starts
            mouseManager.SetPosition(screenCentre);
        }

        private void InitUI()
        {
            SpriteFont spriteFont = fontDictionary["ui"];

            string text = "Score: 0";
            Vector2 originalDimensions = spriteFont.MeasureString(text);

            Transform2D transform2D = new Transform2D(
                new Vector2(originalDimensions.X/2, originalDimensions.Y/2), 0,
                Vector2.One,
                new Vector2(originalDimensions.X / 2, originalDimensions.Y / 2),
                new Integer2(originalDimensions));

            UITextObject score = new UITextObject("score", ActorType.UIText,
                StatusType.Drawn, transform2D,
                Color.Red, 0, SpriteEffects.None,
                text, spriteFont);

            ScoreController controller = new ScoreController("scoreController", ControllerType.Progress, score);
            score.ControllerList.Add(controller);

            uiManager.Add(score);
        }

        private void InitMenu()
        {
            //uncomment and try to instanciate the two UIObjects and add to menu
            /*
            UITextureObject mainBackgroundTextureObject = null;
            UIButtonObject mainPlayButtonObject = null;

            //homework - 27/11/20 - try instanciate a UITextureObject (e.g. mainBackgroundTextureObject to show main menu background) and a UIButtonObject (e.g. mainPlayButtonObject to show play button) and add to the manager

            //add to main menu scene
            menuManager.Add("main", mainBackgroundTextureObject);
            menuManager.Add("main", mainPlayButtonObject);

            //dont forget to say which menu scene you want to be updated and drawn i.e. shown!
            menuManager.SetScene("main");
            */
        }

        private void InitEventDispatcher()
        {
            eventDispatcher = new EventDispatcher(this);
            Components.Add(eventDispatcher);

            EventDispatcher.Subscribe(EventCategoryType.Pickup, HandleEvent);
        }

        private void HandleEvent(EventData eventData)
        {
            if (eventData.EventCategoryType == EventCategoryType.Pickup)
            {
                if (eventData.EventActionType == EventActionType.OnCreate)
                {
                    ArrayList potion_data = eventData.Parameters[0] as ArrayList;
                    string modelName = potion_data[1] as string;
                    ArrayList modelData = GameConstants.pickupModelData[modelName];

                    Transform3D transform3D = new Transform3D(
                        new Vector3(GameConstants.cauldronPos.X, GameConstants.potionSpawnHeight, GameConstants.cauldronPos.Z), //Translation
                        Vector3.Zero,               //Rotation
                        (Vector3) modelData[1], //Scale
                        -Vector3.UnitZ,             //Look
                        Vector3.UnitY);             //Up

                    //effectparameters
                    EffectParameters effectParameters = new EffectParameters(modelEffect,
                        textureDictionary[modelName],
                        Color.White, 1);

                    //model object
                    CollidableObject potionObject = new CollidableObject((string)potion_data[0] + objectManager.NewID(), ActorType.Interactable,
                        StatusType.Drawn | StatusType.Update, transform3D,
                        effectParameters, modelDictionary[modelName]);

                    HandHeldPickup potion = new HandHeldPickup(potionObject, PickupType.Potion, (string)potion_data[0],
                        GameConstants.defualtInteractionDist, (Vector3) modelData[1]);

                    objectManager.Add(potion);
                }
                else if(eventData.EventActionType == EventActionType.OnProcess)
                {
                    string name = eventData.Parameters[0] as string;
                    Vector3 tablePos = (Vector3)eventData.Parameters[1];

                    ArrayList modelData = GameConstants.pickupModelData[name];

                    Transform3D transform3D = new Transform3D(
                        new Vector3(tablePos.X, GameConstants.potionSpawnHeight, tablePos.Z), //Translation + Offset
                        Vector3.Zero,               //Rotation
                        (Vector3)modelData[1],      //Scale
                        -Vector3.UnitZ,             //Look
                        Vector3.UnitY);             //Up

                    //effectparameters
                    EffectParameters effectParameters = new EffectParameters(modelEffect,
                        textureDictionary[name],
                        Color.White, 1);

                    //model object
                    CollidableObject ingredientObject = new CollidableObject(name + objectManager.NewID(), ActorType.Interactable,
                        StatusType.Drawn | StatusType.Update, transform3D,
                        effectParameters, modelDictionary[name]);

                    HandHeldPickup ingredient = new HandHeldPickup(ingredientObject, PickupType.Ingredient, name,
                        GameConstants.defualtInteractionDist, (Vector3)modelData[1], GameConstants.ingredients[name]);

                    objectManager.Add(ingredient);
                }
            }
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
            //stores effects
            effectDictionary = new Dictionary<string, BasicEffect>();

            //stores textures, fonts & models
            modelDictionary = new ContentDictionary<Model>("models", Content);
            textureDictionary = new ContentDictionary<Texture2D>("textures", Content);
            fontDictionary = new ContentDictionary<SpriteFont>("fonts", Content);

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

            //add in-game ui
            //Breaks helper? (will only draw 1 side, the wrong one) spritebatch begin/end?
            uiManager = new UIManager(this, StatusType.Drawn | StatusType.Update, _spriteBatch, 10);
            uiManager.DrawOrder = 4;
            Components.Add(uiManager);

            //add menu
            menuManager = new MenuManager(this, StatusType.Update | StatusType.Drawn, _spriteBatch);
            menuManager.DrawOrder = 5; //highest number of all drawable managers since we want it drawn on top!
            Components.Add(menuManager);
        }

        private void InitCameras3D()
        {
            Transform3D transform3D = null;
            Camera3D camera3D = null;
            Viewport viewPort = new Viewport(0, 0, 1440, 1080);

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

        #endregion Initialization - Managers, Cameras, Effects, Textures

        #region Initialization - Drawn Content

        private void InitCollidableDrawnContent()
        {
            InitStaticCollidableLevel();

            InitStaticCollidableObjects();
        }

        //private void InitStaticCollidableGround()
        //{
        //    CollidableObject collidableObject = null;
        //    Transform3D transform3D = null;
        //    EffectParameters effectParameters = null;
        //    Model model = null;

        //    model = box;

        //    effectParameters = new EffectParameters(modelEffect,
        //          nebula,
        //          Color.White, 1);

        //    transform3D = new Transform3D(Vector3.Zero, Vector3.Zero, new Vector3(worldScale, 1, worldScale), -Vector3.UnitZ, Vector3.UnitY);

        //    collidableObject = new CollidableObject("ground", ActorType.CollidableGround,
        //        StatusType.Update | StatusType.Drawn,
        //        transform3D, effectParameters, model);

        //    //focus on CDCR specific methods and parameters - plane, sphere, box, capsule
        //    collidableObject.AddPrimitive(new JigLibX.Geometry.Plane(transform3D.Up, transform3D.Translation),
        //        new MaterialProperties(0.8f, 0.8f, 0.7f));

        //    collidableObject.Enable(true, 1); //change to false, see what happens.

        //    objectManager.Add(collidableObject);
        //}

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
                textureDictionary["floor"],
                Color.White, 1);

            //model object
            CollidableObject collidableObject = new CollidableObject("floor", ActorType.CollidableDecorator,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, modelDictionary["floor"]);

            collidableObject.AddPrimitive(new Box(new Vector3(-100, 0, -100), Matrix.Identity, new Vector3(800, 53, 800)),
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
                textureDictionary["wall_left"],
                Color.White, 1);

            //model object
            collidableObject = new CollidableObject("wallLeft", ActorType.CollidableDecorator,
               StatusType.Drawn | StatusType.Update, transform3D,
               effectParameters, modelDictionary["wall_left"]);

            collidableObject.AddPrimitive(new Box(new Vector3(-460, 230, -100), Matrix.Identity, new Vector3(110, 430, 800)),
                new MaterialProperties(0.2f, 0.8f, 0.7f));

            collidableObject.Enable(true, 1);

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
                textureDictionary["wall_right"],
                Color.White, 1);

            //model object
            collidableObject = new CollidableObject("wallRight", ActorType.CollidableDecorator,
               StatusType.Drawn | StatusType.Update, transform3D,
               effectParameters, modelDictionary["wall_right"]);

            collidableObject.AddPrimitive(new Box(new Vector3(-100, 230, -460), Matrix.Identity, new Vector3(800, 430, 123)),
                new MaterialProperties(0.2f, 0.8f, 0.7f));

            collidableObject.Enable(true, 1);

            objectManager.Add(collidableObject);


            ///////railing
            //transform 
            transform3D = new Transform3D(new Vector3(0, 0, 0),
                                    new Vector3(0, 0, 0),       //rotation
                                    new Vector3(1, 1, 1),        //scale
                                        -Vector3.UnitZ,         //look
                                        Vector3.UnitY);         //up

            //effectparameters
            effectParameters = new EffectParameters(modelEffect,
                null,
                Color.White, 1);

            //model object
            collidableObject = new CollidableObject("wallRight", ActorType.CollidableDecorator,
               StatusType.Drawn | StatusType.Update, transform3D,
               effectParameters, modelDictionary["railing"]);

            collidableObject.AddPrimitive(new Box(transform3D.Translation, Matrix.Identity, transform3D.Scale * 2),
                new MaterialProperties(0.2f, 0.8f, 0.7f));

            collidableObject.Enable(true, 1);

            objectManager.Add(collidableObject);

            ///////outerWalls
            //transform 
            transform3D = new Transform3D(new Vector3(0, 0, 0),
                                    new Vector3(0, 0, 0),       //rotation
                                    new Vector3(1, 1, 1),        //scale
                                        -Vector3.UnitZ,         //look
                                        Vector3.UnitY);         //up

            //effectparameters
            effectParameters = new EffectParameters(modelEffect,
                null,
                Color.White, 1);

            //model object
            collidableObject = new CollidableObject("outerWalls", ActorType.CollidableDecorator,
               StatusType.Drawn | StatusType.Update, transform3D,
               effectParameters, modelDictionary["outerWalls"]);

            collidableObject.AddPrimitive(new Box(transform3D.Translation, Matrix.Identity, transform3D.Scale * 2),
                new MaterialProperties(0.2f, 0.8f, 0.7f));

            collidableObject.Enable(true, 1);

            objectManager.Add(collidableObject);

            ///////outerWallsRight
            //transform 
            transform3D = new Transform3D(new Vector3(0, 0, 0),
                                    new Vector3(0, 0, 0),       //rotation
                                    new Vector3(1, 1, 1),        //scale
                                        -Vector3.UnitZ,         //look
                                        Vector3.UnitY);         //up

            //effectparameters
            effectParameters = new EffectParameters(modelEffect,
                null,
                Color.White, 1);

            //model object
            collidableObject = new CollidableObject("outer walls right", ActorType.CollidableDecorator,
               StatusType.Drawn | StatusType.Update, transform3D,
               effectParameters, modelDictionary["outerWallsRight"]);

            collidableObject.AddPrimitive(new Box(transform3D.Translation, Matrix.Identity, transform3D.Scale * 2),
                new MaterialProperties(0.2f, 0.8f, 0.7f));

            collidableObject.Enable(true, 1);

            objectManager.Add(collidableObject);

            ///////lava
            //transform 
            transform3D = new Transform3D(new Vector3(0, 0, 0),
                                    new Vector3(0, 0, 0),       //rotation
                                    new Vector3(1, 1, 1),        //scale
                                        -Vector3.UnitZ,         //look
                                        Vector3.UnitY);         //up

            //effectparameters
            effectParameters = new EffectParameters(modelEffect,
                textureDictionary["lava"],
                Color.White, 1);

            //model object
            collidableObject = new CollidableObject("outerWalls", ActorType.CollidableDecorator,
               StatusType.Drawn | StatusType.Update, transform3D,
               effectParameters, modelDictionary["lava"]);

            collidableObject.AddPrimitive(new Box(transform3D.Translation, Matrix.Identity, transform3D.Scale * 2),
                new MaterialProperties(0.2f, 0.8f, 0.7f));

            collidableObject.Enable(true, 1);

            objectManager.Add(collidableObject);
        }

        private void InitStaticCollidableObjects()
        {
            #region Cauldron

            ////////Cauldron
            //transform
            Transform3D transform3D = new Transform3D(GameConstants.cauldronPos,
                                new Vector3(0, 0, 0),       //rotation
                                new Vector3(1, 1, 1),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            //effectparameters
            EffectParameters effectParameters = new EffectParameters(modelEffect,
                textureDictionary["cauldron"],
                Color.White, 1);

            CollidableObject collidableObject = new CollidableObject("Cauldron", ActorType.Interactable,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, modelDictionary["cauldron"]);

            Cauldron cauldron = new Cauldron(collidableObject, "Cauldron",
                GameConstants.defualtInteractionDist, InitStirringMinigame());

            cauldron.AddPrimitive(new Sphere(new Vector3(0, 0, 0), 50), new MaterialProperties(0.2f, 0.8f, 0.7f));
            cauldron.Enable(true, 1);

            objectManager.Add(cauldron);

            #endregion

            #region Bin

            ////////Bin
            //transform
            transform3D = new Transform3D(GameConstants.binPos,
                                new Vector3(0, 0, 0),       //rotation
                                new Vector3(10, 30, 10),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            //effectparameters
            effectParameters = new EffectParameters(modelEffect,
                textureDictionary["crate"],
                Color.White, 1);

            collidableObject = new CollidableObject("Bin", ActorType.Interactable,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, modelDictionary["box2"]);

            Bin bin = new Bin(collidableObject, "Bin", GameConstants.defualtInteractionDist);
            bin.AddPrimitive(new Box(new Vector3(0, 0, 0), Matrix.Identity, transform3D.Scale * 2.14f),
                new MaterialProperties(0.2f, 0.8f, 0.7f));
            bin.Enable(true, 1);

            objectManager.Add(bin);

            #endregion

            #region Grind Table
            ////////////////Grind Table
            /////transform 
            transform3D = new Transform3D(new Vector3(200, 90, -200),
                                new Vector3(0, 0, 0),       //rotation
                                new Vector3(1, 1, 1),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            effectParameters = new EffectParameters(modelEffect,
                textureDictionary["table_grinder"],
                Color.White, 1);

            collidableObject = new CollidableObject("GrindTable", ActorType.Interactable,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, modelDictionary["table_grinder"]);

            IngredientProccessor proccessor = new IngredientProccessor(collidableObject, "Grind Table", 
                GameConstants.defualtInteractionDist, IngredientState.Solid, InitGrindingMinigame());

            proccessor.AddPrimitive(new Box(new Vector3(-5, 0, 0), Matrix.Identity, new Vector3(90, 105, 96)),
                new MaterialProperties(0.2f, 0.8f, 0.7f));

            proccessor.Enable(true, 1);

            objectManager.Add(proccessor);

            #endregion

            #region Liquid Table

            ////////////////Liquid Table
            /////transform 
            transform3D = new Transform3D(new Vector3(175, 35, 0),
                                new Vector3(0, 0, 0),       //rotation
                                new Vector3(1, 1, 1),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            effectParameters = new EffectParameters(modelEffect,
                textureDictionary["table_liquid"],
                Color.White, 1);

            collidableObject = new CollidableObject("LiquidTable", ActorType.Interactable,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, modelDictionary["table_liquid"]);

            proccessor = new IngredientProccessor(collidableObject, "Liquid Table",
                GameConstants.defualtInteractionDist, IngredientState.Dust, InitGrindingMinigame());

            proccessor.AddPrimitive(new Box(new Vector3(15, 60, 0), Matrix.Identity, new Vector3(90, 125, 96)),
                new MaterialProperties(0.2f, 0.8f, 0.7f));

            proccessor.Enable(true, 1);

            objectManager.Add(proccessor);

            #endregion

            #region Lectern

            ////////////////Lectern
            /////transform 
            transform3D = new Transform3D(new Vector3(175, 35, 200),
                                new Vector3(0, 0, 0),       //rotation
                                new Vector3(1, 1, 1),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            effectParameters = new EffectParameters(modelEffect,
                textureDictionary["lectern"],
                Color.White, 1);

            collidableObject = new CollidableObject("Lectern", ActorType.CollidableDecorator,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, modelDictionary["lectern"]);

            collidableObject.AddPrimitive(new Box(new Vector3(0, 50, 0), Matrix.Identity, new Vector3(72, 111, 71)),
                new MaterialProperties(0.2f, 0.8f, 0.7f));

            collidableObject.Enable(true, 1);

            objectManager.Add(collidableObject);

            #endregion

            #region Chest
            ////////////////Chest
            /////transform 
            transform3D = new Transform3D(new Vector3(-370, 80, 110),
                                new Vector3(0, 0, 0),       //rotation
                                new Vector3(1, 1, 1),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            effectParameters = new EffectParameters(modelEffect,
                textureDictionary["chest"],
                Color.White, 1);

            collidableObject = new CollidableObject("Chest", ActorType.CollidableDecorator,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, modelDictionary["chest"]);

            collidableObject.AddPrimitive(new Box(new Vector3(0, 0, 0), Matrix.Identity, new Vector3(75, 94, 112)),
                new MaterialProperties(0.2f, 0.8f, 0.7f));

            collidableObject.Enable(true, 1);

            objectManager.Add(collidableObject);

            #endregion

            #region Red Rock

            ///////////////////////////////////Red Rock Giver\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            //transform 
            transform3D = new Transform3D(new Vector3(-350, 10, -250),
                                new Vector3(0, 0, 0),       //rotation
                                (Vector3)GameConstants.pickupModelData["Red_Solid"][1], //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            //effectparameters
            effectParameters = new EffectParameters(modelEffect,
                textureDictionary["Red_Solid"],
                Color.Red, 1);

            //model object
            collidableObject = new CollidableObject("RedRock", ActorType.Interactable,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, modelDictionary["Red_Solid"]);

            //Red rock pickup
            HandHeldPickup pickup = new HandHeldPickup(collidableObject, PickupType.Ingredient, "Red Rock",
                GameConstants.defualtInteractionDist, GameConstants.potionRedPos, GameConstants.ingredients["Red_Solid"]);

            pickup.AddPrimitive(new Box(Vector3.Zero, Matrix.Identity, transform3D.Scale),
                new MaterialProperties(0.2f, 0.8f, 0.7f));

            #endregion Red Rock

            #region Red Giver

            ////////////////Giver creation
            /////transform 
            transform3D = new Transform3D(new Vector3(-350, 10, -250),
                                new Vector3(0, 0, 0),       //rotation
                                new Vector3(0.7f, 0.7f, 0.7f),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            effectParameters = new EffectParameters(modelEffect,
                textureDictionary["table_red"],
                Color.White, 1);

            collidableObject = new CollidableObject("RedRockGiver", ActorType.Interactable,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, modelDictionary["table_red"]);



            IngredientGiver ingredientGiver = new IngredientGiver(collidableObject, "Red Rock Giver",
                GameConstants.defualtInteractionDist, pickup);

            ingredientGiver.AddPrimitive(new Box(new Vector3(-15, 50, 12), Matrix.Identity, new Vector3(80, 83, 140)),
                new MaterialProperties(0.2f, 0.8f, 0.7f));

            ingredientGiver.Enable(true, 1);

            objectManager.Add(ingredientGiver);

            #endregion Red Giver

            #region Blue Flower

            ///////////////////////////////////Blue Flower Giver\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            //transform 
            transform3D = new Transform3D(new Vector3(-200, 10, -370),
                                new Vector3(0, 90, 0),       //rotation
                                (Vector3)GameConstants.pickupModelData["Blue_Solid"][1],        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            //effectparameters
            effectParameters = new EffectParameters(modelEffect,
                textureDictionary["Blue_Solid"],
                Color.White, 1);

            //model object
            collidableObject = new CollidableObject("BlueFlower", ActorType.Interactable,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, modelDictionary["Blue_Solid"]);

            //Blue flower pickup
            pickup = new HandHeldPickup(collidableObject, PickupType.Ingredient, "Blue Flower",
                GameConstants.defualtInteractionDist, GameConstants.potionRedPos, GameConstants.ingredients["Blue_Solid"]);

            pickup.AddPrimitive(new Box(Vector3.Zero, Matrix.Identity, transform3D.Scale),
                new MaterialProperties(0.2f, 0.8f, 0.7f));

            #endregion Blue Flower

            #region Blue Giver

            ////////////////Giver Creation
            //transform 
            transform3D = new Transform3D(new Vector3(-200, 10, -370),
                                new Vector3(0, 0, 0),       //rotation
                                new Vector3(0.7f, 0.7f, 0.7f),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            effectParameters = new EffectParameters(modelEffect,
                textureDictionary["table_blue"],
                Color.White, 1);

            collidableObject = new CollidableObject("BlueFlowerGiver", ActorType.Interactable,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, modelDictionary["table_blue"]);

            ingredientGiver = new IngredientGiver(collidableObject, "Blue Flower Giver",
                GameConstants.defualtInteractionDist, pickup);

            ingredientGiver.AddPrimitive(new Box(new Vector3(12, 50, 0), Matrix.Identity, new Vector3(140, 83, 80)),
                new MaterialProperties(0.2f, 0.8f, 0.7f));

            ingredientGiver.Enable(true, 1);

            objectManager.Add(ingredientGiver);

            #endregion Blue Giver

            #region Green Herb

            ///////////////////////////////////Green Herb Giver\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            //transform 
            transform3D = new Transform3D(new Vector3(100, 30, -350),
                                new Vector3(0, 90, 0),       //rotation
                                (Vector3)GameConstants.pickupModelData["Green_Solid"][1], //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            //effectparameters
            effectParameters = new EffectParameters(modelEffect,
                textureDictionary["Green_Solid"],
                Color.White, 1);

            //model object
            collidableObject = new CollidableObject("GreenMushroom", ActorType.Interactable,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, modelDictionary["Green_Solid"]);

            pickup = new HandHeldPickup(collidableObject, PickupType.Ingredient, "Green Mushroom",
                GameConstants.defualtInteractionDist, GameConstants.potionRedPos, GameConstants.ingredients["Green_Solid"]);

            pickup.AddPrimitive(new Box(Vector3.Zero, Matrix.Identity, transform3D.Scale),
                new MaterialProperties(0.2f, 0.8f, 0.7f));

            #endregion Green Herb

            #region Green Giver

            ////////////////Giver creation
            //transform 
            transform3D = new Transform3D(new Vector3(100, 30, -350),
                                new Vector3(0, 90, 0),       //rotation
                                new Vector3(0.5f, 0.5f, 0.5f),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            effectParameters = new EffectParameters(modelEffect,
                textureDictionary["table_green"],
                Color.White, 1);

            collidableObject = new CollidableObject("GreenHerbGiver", ActorType.Interactable,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, modelDictionary["table_green"]);

            ingredientGiver = new IngredientGiver(collidableObject, "Green Herb Giver",
                GameConstants.defualtInteractionDist, pickup);

            ingredientGiver.AddPrimitive(new Box(new Vector3(0, 40, 0), Matrix.Identity, new Vector3(150, 83, 80)),
                new MaterialProperties(0.2f, 0.8f, 0.7f));

            ingredientGiver.Enable(true, 1);

            objectManager.Add(ingredientGiver);

            #endregion Green Giver

        }

        #region Init Minigames

        private StirringMinigameController InitStirringMinigame()
        {
            Texture2D texture = textureDictionary["ring"];

            Transform2D transform2D = new Transform2D(screenCentre, 0,
                Vector2.One,
                new Vector2(texture.Width / 2, texture.Height / 2),
                new Integer2(texture.Width, texture.Height));

            UITextureObject background = new UITextureObject("ring", ActorType.UITextureObject,
                StatusType.Off, transform2D, Color.White, 30, SpriteEffects.None, texture,
                new Microsoft.Xna.Framework.Rectangle(0, 0, texture.Width, texture.Height));

            texture = textureDictionary["ball"];

            transform2D = new Transform2D(screenCentre, 0,
                Vector2.One,
                new Vector2(texture.Width / 2, texture.Height / 2),
                new Integer2(texture.Width, texture.Height));

            UITextureObject ball = new UITextureObject("ball", ActorType.UITextureObject,
                StatusType.Off, transform2D, Color.White, 30, SpriteEffects.None, texture,
                new Microsoft.Xna.Framework.Rectangle(0, 0, texture.Width, texture.Height));

            float radius = background.Texture.Width / 2 - ball.Texture.Width / 4;

            uiManager.Add(background);
            uiManager.Add(ball);

            return new StirringMinigameController("StirringMinigame",
                ActorType.Decorator, StatusType.Off, mouseManager, background, radius, ball);
        }

        private GrindingMinigameController InitGrindingMinigame()
        {
            Texture2D texture = textureDictionary["Aidan'sPotion"];

            Transform2D transform2D = new Transform2D(screenCentre, 0,
                Vector2.One,
                new Vector2(texture.Width / 2, texture.Height / 2),
                new Integer2(texture.Width, texture.Height));

            UITextureObject ui = new UITextureObject("Aidan'sPotion", ActorType.UITextureObject,
                StatusType.Off, transform2D, Color.White, 30, SpriteEffects.None, texture,
                new Microsoft.Xna.Framework.Rectangle(0, 0, texture.Width, texture.Height));

            uiManager.Add(ui);

            return new GrindingMinigameController("GrindingMinigame",
                ActorType.Decorator, StatusType.Off, keyboardManager, gamePadManager, ui);
        }

        #endregion

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
        }

        private void InitStaticModels()
        {

        }

        private void InitPlayer()
        {
            //transform
            Transform3D transform3D = new Transform3D(new Vector3(0, 50, 0),
                                new Vector3(0, 0, 0),       //rotation
                                new Vector3(1, 1, 1),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            //effectparameters
            EffectParameters effectParameters = new EffectParameters(modelEffect,
                textureDictionary["wizard"],
                Color.White, 1);

            //model object
            ModelObject playerObject = new ModelObject("Wizard", ActorType.Player,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, modelDictionary["wizard"]);

            ThirdPersonPlayerController controller = new ThirdPersonPlayerController(
                "3rd person player controller", ControllerType.ThirdPerson,
                keyboardManager, mouseManager, cameraManager[0],
                GameConstants.playerMoveSpeed,
                GameConstants.playerRotateSpeed,
                GameConstants.MoveKeys);

            //interact helper
            PrimitiveObject interactHelper = archetypalTexturedQuad.Clone() as PrimitiveObject;
            interactHelper.ID = "spacebar helper";
            interactHelper.EffectParameters.Texture = textureDictionary["helper_space"];
            interactHelper.StatusType = StatusType.Drawn;
            interactHelper.ActorType = ActorType.Decorator;
            interactHelper.Transform3D.Scale = new Vector3(80, 56, 0);
            interactHelper.Transform3D.RotationInDegrees = new Vector3(-45, 0, 0);

            objectManager.Add(interactHelper);

            Player player = new Player(playerObject, 20, 1, 2, 2,
                objectManager, keyboardManager, gamePadManager, controller, interactHelper);

            player.Enable(false, 1);

            objectManager.Add(player);

            persistantData = new PersistantData();
        }

        #endregion Initialization - Drawn Content

        #region Vertices, helpers and skybox

        private void InitPrimitiveArchetypes() //formerly InitTexturedQuad
        {
            Transform3D transform3D = new Transform3D(Vector3.Zero, Vector3.Zero,
               Vector3.One, Vector3.UnitZ, Vector3.UnitY);

            EffectParameters effectParameters = new EffectParameters(unlitTexturedEffect,
                textureDictionary["grass1"], /*bug*/ Color.White, 1);

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
            primitiveObject.EffectParameters.Texture = textureDictionary["sky_back"]; ;
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            primitiveObject.Transform3D.Translation = new Vector3(0, 0, -worldScale / 2.0f);
            objectManager.Add(primitiveObject);

            //left
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            primitiveObject.ID = "left back";
            primitiveObject.EffectParameters.Texture = textureDictionary["sky_left"]; ;
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, 90, 0);
            primitiveObject.Transform3D.Translation = new Vector3(-worldScale / 2.0f, 0, 0);
            objectManager.Add(primitiveObject);

            //right
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            primitiveObject.ID = "sky right";
            primitiveObject.EffectParameters.Texture = textureDictionary["sky_right"];
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 20);
            primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, -90, 0);
            primitiveObject.Transform3D.Translation = new Vector3(worldScale / 2.0f, 0, 0);
            objectManager.Add(primitiveObject);

            //top
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            primitiveObject.ID = "sky top";
            primitiveObject.EffectParameters.Texture = textureDictionary["sky"];
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            primitiveObject.Transform3D.RotationInDegrees = new Vector3(90, -90, 0);
            primitiveObject.Transform3D.Translation = new Vector3(0, worldScale / 2.0f, 0);
            objectManager.Add(primitiveObject);

            //front
            primitiveObject = archetypalTexturedQuad.Clone() as PrimitiveObject;
            primitiveObject.ID = "sky front";
            primitiveObject.EffectParameters.Texture = textureDictionary["sky_front"];
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, 180, 0);
            primitiveObject.Transform3D.Translation = new Vector3(0, 0, worldScale / 2.0f);
            objectManager.Add(primitiveObject);
        }

        #endregion Vertices, helpers and skybox

        #region Load & Unload Game Assets

        protected override void LoadContent()
        {
        }

        protected override void UnloadContent()
        {
            //housekeeping - unload content
            textureDictionary.Dispose();
            modelDictionary.Dispose();
            fontDictionary.Dispose();
            modelDictionary.Dispose();

            base.UnloadContent();
        }

        #endregion Load & Unload Game Assets

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

            if (keyboardManager.IsFirstKeyPress(Keys.F2))
            {
                if (isPaused) //menu -> game
                {
                    EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnPause,
                        new object[] { gameTime }));
                }
                else //game -> menu
                {
                    EventDispatcher.Publish(new EventData(EventCategoryType.Menu, EventActionType.OnPlay,
                    new object[] { gameTime }));
                }
                isPaused = !isPaused;
            }

            if (keyboardManager.IsFirstKeyPress(Keys.C))
            {
                cameraManager.CycleActiveCamera();
            }

            if (keyboardManager.IsFirstKeyPress(Keys.F1))
            {
                if (debugInfoDrawer.Visible)
                {
                    debugInfoDrawer.Visible = false;
                    physicsDebugDrawer.Visible = false;
                }
                else
                {
                    debugInfoDrawer.Visible = true;
                    physicsDebugDrawer.Visible = true;
                }

            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            _graphics.GraphicsDevice.DepthStencilState = dss;

            _spriteBatch.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            base.Draw(gameTime);
        }

        #endregion Update & Draw
    }
}