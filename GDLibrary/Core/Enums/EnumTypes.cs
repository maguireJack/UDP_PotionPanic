namespace GDLibrary.Enums
{
    /// <summary>
    /// Actor types within the game (both drawn and undrawn)
    /// </summary>
    public enum ActorType : sbyte
    {
        NonPlayer,
        Player,    //hero (rendered using Max/Maya file)
        Decorator, //architecture, obstacle (rendered using Max/Maya file)
        Primitive, //make this type using IVertexData
        Pickups,
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
}