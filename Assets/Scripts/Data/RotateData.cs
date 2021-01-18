using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct RotateData : IComponentData
{
    public float3 rotation;
    public float turnSpeed;
}