using Unity.Entities;

public class LevelScene
{
    /// Data Containers
    public struct Data : IComponentData
    {
        public SceneDefinition sceneDefinition;
        public Hash128 sceneGUID;
    }

    /// Tag components
    public struct Unused : IComponentData { }
    public struct Initialized : IComponentData { }
    public struct Active : IComponentData { }
    public struct InTransition : IComponentData { }

    public class Loader
    {
        // Tag components
        public struct DoPreload : IComponentData { }
        public struct DoTransition : IComponentData { }
        public struct DoCleanup : IComponentData { }
        public struct DoLoad : IComponentData { }
        public struct DoUnload : IComponentData { }
    }
}


