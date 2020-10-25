using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GDGame;

namespace GDLibrary
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private BasicEffect unlitTexturedEffect, unlitWireframeEffect;
        private CameraManager cameraManager;
        private ObjectManager objectManager;
        private KeyboardManager keyboardManager;
        private MouseManager mouseManager;

        //eventually we will remove this content
        private VertexPositionColorTexture[] vertices;
        private Texture2D backSky, leftSky, rightSky, frontSky, topSky, grass;
        private Model cargo, wizard, redPotion;
        private PrimitiveObject archetypalTexturedQuad;
        private float worldScale = 3000;
        PrimitiveObject primitiveObject = null;
        Vector2 screenCentre = Vector2.Zero;
        private BasicEffect modelEffect;
        private SpriteFont debugFont;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        #region Initialization - Managers, Cameras, Effects, Textures
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Window.Title = "My Amazing Game";

            //camera
            this.cameraManager = new CameraManager(this);
            Components.Add(this.cameraManager);

            //keyboard
            this.keyboardManager = new KeyboardManager(this);
            Components.Add(this.keyboardManager);

            //mouse
            this.mouseManager = new MouseManager(this, false);
            Components.Add(this.mouseManager);

            InitCameras3D();
            InitManagers();
            InitVertices();
            InitTextures();
            InitFonts();
            InitEffect();
            InitDrawnContent();

            InitGraphics(1024, 768);

            base.Initialize();
        }

        private void InitDebug()
        {
            Components.Add(new DebugDrawer(this, _spriteBatch, this.debugFont,
                this.cameraManager, this.objectManager));

        }

        private void InitFonts()
        {
            this.debugFont = Content.Load<SpriteFont>("Assets/Fonts/debug");
        }

        private void InitManagers()
        {
            this.objectManager = new ObjectManager(this, 6, 10, this.cameraManager);
            Components.Add(this.objectManager);
        }

        private void InitCameras3D()
        {
            Transform3D transform3D = null;
            Camera3D camera3D = null;

            #region Camera - Third Person Player Camera
            transform3D = new Transform3D(new Vector3(
                0,
                GConstants.PlayerCamOffsetY,
                GConstants.PlayerCamOffsetZ),
                new Vector3(0, 0, -1), Vector3.UnitY);

            camera3D = new Camera3D("3rd person player",
                ActorType.Camera3D, StatusType.Update, transform3D,
                ProjectionParameters.StandardDeepSixteenTen);
            this.cameraManager.Add(camera3D);
            #endregion

            #region Camera - First Person
            transform3D = new Transform3D(new Vector3(10, 10, 20),
                new Vector3(0, 0, -1), Vector3.UnitY);

            camera3D = new Camera3D("1st person",
                ActorType.Camera3D, StatusType.Update, transform3D,
                ProjectionParameters.StandardDeepSixteenTen);

            //attach a controller
            camera3D.ControllerList.Add(new FirstPersonCameraController(
                this.keyboardManager, this.mouseManager,
                GDConstants.moveSpeed, GDConstants.strafeSpeed, GDConstants.rotateSpeed));
            this.cameraManager.Add(camera3D);
            #endregion

            #region Camera - Flight
            transform3D = new Transform3D(new Vector3(0, 10, 10),
                        new Vector3(0, 0, -1),
                        Vector3.UnitY);

            camera3D = new Camera3D("flight person",
                ActorType.Camera3D, StatusType.Update, transform3D,
                ProjectionParameters.StandardDeepSixteenTen);

            //attach a controller
            camera3D.ControllerList.Add(new FlightCameraController(
                this.keyboardManager, this.mouseManager,
                GDConstants.moveSpeed, GDConstants.strafeSpeed, GDConstants.rotateSpeed));
            this.cameraManager.Add(camera3D);
            #endregion

            #region Camera - Security
            transform3D = new Transform3D(new Vector3(10, 10, 50),
                        new Vector3(0, 0, -1),
                        Vector3.UnitY);

            camera3D = new Camera3D("security",
                ActorType.Camera3D, StatusType.Update, transform3D,
            ProjectionParameters.StandardDeepSixteenTen);

            camera3D.ControllerList.Add(new PanController(new Vector3(1, 1, 0),
                                            30, GDConstants.mediumAngularSpeed, 0));
            this.cameraManager.Add(camera3D);
            #endregion

            #region Camera - Giant
            transform3D = new Transform3D(new Vector3(0, 250, 100),
                       new Vector3(0, -1, -1), //look
                       new Vector3(0, 1, -1)); //up

            this.cameraManager.Add(new Camera3D("giant looking down 1st person",
              ActorType.Camera3D, StatusType.Update, transform3D,
          ProjectionParameters.StandardDeepSixteenTen));
            this.cameraManager.Add(camera3D);
            #endregion

            this.cameraManager.ActiveCameraIndex = 0; //0, 1, 2, 3

        }

        private void InitEffect()
        {
            //to do...
            this.unlitTexturedEffect = new BasicEffect(this._graphics.GraphicsDevice);
            this.unlitTexturedEffect.VertexColorEnabled = true; //otherwise we wont see RGB
            this.unlitTexturedEffect.TextureEnabled = true;

            //wireframe primitives with no lighting and no texture
            this.unlitWireframeEffect = new BasicEffect(this._graphics.GraphicsDevice);
            this.unlitWireframeEffect.VertexColorEnabled = true;

            //model effect
            //add a ModelObject
            this.modelEffect = new BasicEffect(this._graphics.GraphicsDevice);
            this.modelEffect.TextureEnabled = true;
            this.modelEffect.LightingEnabled = true;
            //this.modelEffect.PreferPerPixelLighting = true;
            //this.modelEffect.SpecularPower = 256;
            //this.modelEffect.SpecularColor = Color.Red.ToVector3();
            this.modelEffect.EnableDefaultLighting();
        }

        private void InitTextures()
        {
            //step 1 - texture
            this.backSky
                = Content.Load<Texture2D>("Assets/Textures/Skybox/back");
            this.leftSky
               = Content.Load<Texture2D>("Assets/Textures/Skybox/left");
            this.rightSky
              = Content.Load<Texture2D>("Assets/Textures/Skybox/right");
            this.frontSky
              = Content.Load<Texture2D>("Assets/Textures/Skybox/front");
            this.topSky
              = Content.Load<Texture2D>("Assets/Textures/Skybox/sky");

            this.grass
              = Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1");

            this.cargo
              = Content.Load<Model>("Assets/Models/Cargo");

            this.wizard
              = Content.Load<Model>("Assets/Models/wizard");

            this.redPotion = Content.Load<Model>("Assets/Models/potion1");
        }
        #endregion

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

            InitPlayer();
            initProps();
        }

        private void initProps() 
        {
            //transform 
            Transform3D transform3D = new Transform3D(new Vector3(10, 10, 10),
                                new Vector3(0, 0, 0),       //rotation
                                new Vector3(1, 1, 1),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            //effectparameters
            EffectParameters effectParameters = new EffectParameters(modelEffect,
                null,
                Color.White, 1);

            //model
            Model model = Content.Load<Model>("Assets/Models/potion1");

            //model object
            ModelObject potionObject = new ModelObject("potion1", ActorType.Pickups,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, model);

            InteractableObject potion1 = new InteractableObject(potionObject, "red Potion", 1f);
            this.objectManager.Add(potion1);
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
                null,
                Color.White, 1);

            //model
            Model model = Content.Load<Model>("Assets/Models/wizard");

            //model object
            ModelObject playerObject = new ModelObject("Wizard", ActorType.Player,
                StatusType.Drawn | StatusType.Update, transform3D,
                effectParameters, model);

            ThirdPersonPlayerController controller = new ThirdPersonPlayerController(
                keyboardManager, mouseManager, cameraManager.ActiveCamera,
                GConstants.PlayerMoveSpeed,
                GConstants.PlayerStrafeSpeed,
                GConstants.PlayerRotateSpeed,
                GConstants.PlayerMoveKeysOne);

            Player player = new Player(playerObject, controller);

            this.objectManager.Add(player);
        }

        private void InitStaticModels()
        {
            //transform
            Transform3D transform3D = new Transform3D(new Vector3(0, 5, 0),
                                new Vector3(0, 0, 45),       //rotation
                                new Vector3(1, 1, 1),        //scale
                                    -Vector3.UnitZ,         //look
                                    Vector3.UnitY);         //up

            //effectparameters
            EffectParameters effectParameters = new EffectParameters(modelEffect,
                Content.Load<Texture2D>("Assets/Textures/Props/Crates/crate1"),
                Color.White, 1);

            //model
            Model model = Content.Load<Model>("Assets/Models/box2");
        }

        private void InitVertices()
        {
            this.vertices
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

            EffectParameters effectParameters = new EffectParameters(this.unlitTexturedEffect,
                this.grass, /*bug*/ Color.White, 1);

            IVertexData vertexData = new VertexData<VertexPositionColorTexture>(
                this.vertices, PrimitiveType.TriangleStrip, 2);

            this.archetypalTexturedQuad = new PrimitiveObject("original texture quad",
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

            EffectParameters effectParameters = new EffectParameters(this.unlitWireframeEffect,
                null, Color.White, 1);

            //at this point, we're ready!
            PrimitiveObject primitiveObject = new PrimitiveObject("origin helper",
                ActorType.Helper, StatusType.Drawn, transform3D, effectParameters, vertexData);

            this.objectManager.Add(primitiveObject);

        }

        private void InitSkybox()
        {
            //back
            primitiveObject = this.archetypalTexturedQuad.Clone() as PrimitiveObject;
            //  primitiveObject.StatusType = StatusType.Off; //Experiment of the effect of StatusType
            primitiveObject.ID = "sky back";
            primitiveObject.EffectParameters.Texture = this.backSky;
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            primitiveObject.Transform3D.Translation = new Vector3(0, 0, -worldScale / 2.0f);
            this.objectManager.Add(primitiveObject);

            //left
            primitiveObject = this.archetypalTexturedQuad.Clone() as PrimitiveObject;
            primitiveObject.ID = "left back";
            primitiveObject.EffectParameters.Texture = this.leftSky;
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, 90, 0);
            primitiveObject.Transform3D.Translation = new Vector3(-worldScale / 2.0f, 0, 0);
            this.objectManager.Add(primitiveObject);

            //right
            primitiveObject = this.archetypalTexturedQuad.Clone() as PrimitiveObject;
            primitiveObject.ID = "sky right";
            primitiveObject.EffectParameters.Texture = this.rightSky;
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 20);
            primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, -90, 0);
            primitiveObject.Transform3D.Translation = new Vector3(worldScale / 2.0f, 0, 0);
            this.objectManager.Add(primitiveObject);


            //top
            primitiveObject = this.archetypalTexturedQuad.Clone() as PrimitiveObject;
            primitiveObject.ID = "sky top";
            primitiveObject.EffectParameters.Texture = this.topSky;
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            primitiveObject.Transform3D.RotationInDegrees = new Vector3(90, -90, 0);
            primitiveObject.Transform3D.Translation = new Vector3(0, worldScale / 2.0f, 0);
            this.objectManager.Add(primitiveObject);

            //to do...front
            primitiveObject = this.archetypalTexturedQuad.Clone() as PrimitiveObject;
            primitiveObject.ID = "sky front";
            primitiveObject.EffectParameters.Texture = this.frontSky;
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            primitiveObject.Transform3D.RotationInDegrees = new Vector3(0, 180, 0);
            primitiveObject.Transform3D.Translation = new Vector3(0, 0, worldScale / 2.0f);
            this.objectManager.Add(primitiveObject);

        }

        private void InitGround()
        {
            //grass
            primitiveObject = this.archetypalTexturedQuad.Clone() as PrimitiveObject;
            primitiveObject.ID = "grass";
            primitiveObject.EffectParameters.Texture = this.grass;
            primitiveObject.Transform3D.Scale = new Vector3(worldScale, worldScale, 1);
            primitiveObject.Transform3D.RotationInDegrees = new Vector3(90, 90, 0);
            this.objectManager.Add(primitiveObject);
        }

        private void InitGraphics(int width, int height)
        {
            //set resolution
            this._graphics.PreferredBackBufferWidth = width;
            this._graphics.PreferredBackBufferHeight = height;

            //dont forget to apply resolution changes otherwise we wont see the new WxH
            this._graphics.ApplyChanges();

            //set screen centre based on resolution
            this.screenCentre = new Vector2(width / 2, height / 2);

            //set cull mode to show front and back faces - inefficient but we will change later
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            this._graphics.GraphicsDevice.RasterizerState = rs;

            //we use a sampler state to set the texture address mode to solve the aliasing problem between skybox planes
            SamplerState samplerState = new SamplerState();
            samplerState.AddressU = TextureAddressMode.Clamp;
            samplerState.AddressV = TextureAddressMode.Clamp;
            this._graphics.GraphicsDevice.SamplerStates[0] = samplerState;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            InitDebug();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }
        #endregion

        #region Update & Draw
        protected override void Update(GameTime gameTime)
        {
            if (this.keyboardManager.IsFirstKeyPress(Keys.Escape))
                Exit();

            if (this.keyboardManager.IsFirstKeyPress(Keys.C))
                this.cameraManager.CycleActiveCamera();
            // this.cameraManager.ActiveCameraIndex++;

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);

        }

        #endregion
    }
}