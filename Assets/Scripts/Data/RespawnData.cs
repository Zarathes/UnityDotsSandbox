using Unity.Entities;

[GenerateAuthoringComponent]
public struct RespawnData : IComponentData
{
    public Entity spawnerReference;
}
