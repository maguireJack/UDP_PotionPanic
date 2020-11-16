namespace GDLibrary.Enums
{
    public enum ScreenLayoutType : sbyte
    {
        Single,
        Multi
    }

    public enum DrawType : sbyte
    {
        Opaque,
        Semitransparent
    }

    /// <summary>
    /// Actor types within the game (both drawn and undrawn)
    /// </summary>
    public enum ActorType : sbyte
    {
        NonPlayer,
        Player,    //hero (rendered using Max/Maya file)
        Decorator, //architecture, obstacle (rendered using Max/Maya file)
        Primitive, //make this type using IVertexData
        Pickup,
        Interactable,

        Camera2D,
        Camera3D,

        Helper
    }

    /// <summary>
    /// Possible status types for an actor within the game (e.g. Update | Drawn, Update, Drawn, Off)
    /// </summary>
    public enum StatusType
    {
        //used for enabling objects for updating and drawing e.g. a model or a camera, or a controller
        Drawn = 1,

        Update = 2,
        Off = 0, //neither drawn, nor updated e.g. the objectmanager when the menu is shown at startup - see Main::InitializeManagers()

        /*
        * Q. Why do we use powers of 2? Will it allow us to do anything different?
        * A. StatusType.Updated | StatusType.Drawn - See ObjectManager::Update() or Draw()
        */
    }

    /// <summary>
    /// Controller types to be applied to an actor (both drawn and undrawn) within the game
    /// </summary>
    public enum ControllerType
    {
        //camera specific
        FlightCamera,

        ThirdPerson,

        //applied to any Actor3D
        FirstPerson,

        Pan,
        Rail,
        Curve,

        AlphaCycle,
        SinTranslation
    }

    /// <summary>
    /// Alignment plane types for a surface within the game (e.g. a primitive object, such as a circle, is aligned with the XY plane)
    /// </summary>
    public enum AlignmentPlaneType : sbyte
    {
        XY,
        XZ,
        YZ
    }

    /// <summary>
    /// Event categories within the game that a subscriber can subscribe to in the EventDispatcher
    /// </summary>
    /// <see cref="GDLibrary.Events.EventData"/>
    /// <seealso cref="GDLibrary.Events.EventDispatcher_OLD"/>
    public enum EventCategoryType
    {
        Camera,
        Player,
        NonPlayer,
        Pickup,
        Sound,
        Menu,
        UI,
        Object,
        Interactable
        //add more here...
    }

    /// <summary>
    /// Event actions that can occur within a category (e.g. EventCategoryType.Sound with EventActionType.OnPlay)
    /// </summary>
    /// <see cref="GDLibrary.Events.EventData"/>
    /// <seealso cref="GDLibrary.Events.EventDispatcher_OLD"/>
    public enum EventActionType
    {
        //sent by audio, video
        OnPlay,
        OnPause,
        OnResume,
        OnStop,
        OnStopAll,

        //processed by many managers (incl. menu, sound, object, ui, physic) and video controller
        OnStart,
        OnRestart,
        OnVolumeDelta,
        OnVolumeSet,
        OnMute,
        OnUnMute,
        OnExit,

        //send by mouse or gamepad manager
        OnClick,
        OnHover,

        //sent by camera manager
        OnCameraSetActive,
        OnCameraCycle,

        //sent by player when gains or loses health
        OnHealthDelta,

        //sent to set player health to a specific start/end value
        OnHealthSet,

        //sent by game state manager
        OnLose,
        OnWin,
        OnPickup,

        //sent whenever we change the opacity of a drawn object - remember ObjectManager has two draw lists (opaque and transparent)
        OnOpaqueToTransparent,
        OnTransparentToOpaque,

        //sent when we want to add/remove an Actor from the game - see ObjectManager::Remove()
        OnAddActor,
        OnRemoveActor,
        OnSpawn,

        OnCreate,

        //sent to interactable actors to prevent them from being interacted with
        OnLock,
        OnUnlock

        //add more here...
    }

}