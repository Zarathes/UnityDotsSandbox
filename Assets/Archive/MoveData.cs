using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]

public struct MoveData : IComponentData
{
    public float3 direction;
    public float movementSpeed;
    public float turnSpeed;
}
