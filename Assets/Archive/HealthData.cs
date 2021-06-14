using Unity.Entities;

[GenerateAuthoringComponent]
public struct HealthData : IComponentData
{
    public bool isDead;
    public float healthPoints;
    public float maxHealthPoints;
}
