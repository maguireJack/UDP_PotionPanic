namespace GDLibrary
{
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
}
