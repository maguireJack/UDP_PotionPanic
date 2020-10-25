namespace GDLibrary
{
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
}