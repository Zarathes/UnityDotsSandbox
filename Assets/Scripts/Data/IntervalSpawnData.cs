using Unity.Entities;

[GenerateAuthoringComponent]
public struct IntervalSpawnerData : IComponentData
{
    public Entity spawnPrefab;
    public float cooldown; 
    public float interval;
}
