using Unity.Entities;

[GenerateAuthoringComponent]
public struct InitializeRespawnData : IComponentData
{
    public Entity spawnPrefab;
}
