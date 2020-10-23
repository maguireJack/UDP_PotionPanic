**For a markdown cheat sheet see [Markdown Cheat Sheet](https://www.markdownguide.org/cheat-sheet/)**

## 3D Game Engine Development - [Intro to MonoGame](https://github.com/nmcguinness/GD_IntroToMonoGame.git)

### Code to Explain
- [ ] Explain effect of SamplerState on on-screen aliasing
- [ ] Explain and use VertexFactory
- [ ] Explain new input manager classes (MouseManager, KeyboardManager, GamePadManager)
- [x] Explain and use IController and the classes which implement it.
- [x] Create a simple enum using 2^N values on the enum values and demonstrate bitwise operators.
- [ ] Explain ModelObject

### Refactor for Efficiency
- [ ] Improve efficiency of ProjectionParameters::Projection property by adding isDirty flag
- [ ] Improve efficiency of Transform3D::World property by adding isDirty flag
- [ ] Improve efficiency of Camera3D::View property by adding isDirty flag
- [ ] Add validation to appropriate set properties

### Tasks - Week 2 
- [x] Introduces view, projection, effect, and VertexPositionColor concepts to render a wireframe triangle to the screen.
- [x] Added a VertexData class to draw VertexPositionColor vertex types.

### Tasks - Week 3
- [x] Added ProjectionParamters to encapsulate projection matrix.
- [x] Added assets to the Content.mgcb file. See [MonoGame Tutorial: Textures and SpriteBatch](https://gamefromscratch.com/monogame-tutorial-textures-and-spritebatch/)
- [x] Rename default namespace to GDLibrary
- [x] Add generic class for VertexData
- [x] Added folder system and organised existing files
- [x] Add EffectParameters
- [x] Add PrimitiveObject
- [x] Add SkyBox
- [x] Add Transform3D
- [x] Add IActor
- [x] Add Actor3D
- [x] Add Camera3D 
- [x] Add PrimitiveObject
- [x] Organise new classes into folder structure

### Tasks - Week 4
- [x] Added new enums: ActorType and StatusType
- [x] Add Actor::Description, Actor::ActorType, and Actor::StatusType
- [x] Add Clone, GetHashCode, Equals to classes in IActor hierarchy
- [x] Add ObjectManager and created lists using DrawnActor3D
- [x] Removed unnecessary GetAlpha() etc from IActor after change to ObjectManager list from IActor to DrawnActor3D
- [x] Add CameraManager and make a GameComponent
- [x] Added use of input managers (mouse, keyboard)
- [x] Re-factor IActor::Draw and ObjectManager to use CameraManager
- [x] Use StatusType in ObjectManager Update and Draw
- [x] Added subfolders to Actor folder for drawn and camera actors
- [ ] Add ContentDictionary
- [ ] Add tiling functionality (see grass plane)
- [x] Add IController and ControllerList to Actor
- [ ] Add 1st, Flight, Pan (Security), 3rd, and Rail camera controllers
- [ ] Add ModelObject
- [ ] Add GDConstants and move "magic-number" hard-coded values
- [ ] Remove hard-coded (512, 384) in Controller and replace with screenCentre
