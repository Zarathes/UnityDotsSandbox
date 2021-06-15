// Author: Bart Schut
using Unity.Entities;
using Unity.Mathematics;

public struct FireFlySettings : IComponentData
{
    public Entity habitatReference;
    public float fireflySpeed;

    public float cycleDuration;
    public float cycleLitDuration;
    public float cycleDormantBrightness;
    public float cycleLitBrightness;
    public float neighborInfluenceRadius;
    public float neighborInfluenceDelta;
    public float fireflyDormantScale;
    public float fireflyLitScale;

    public static FireFlySettings Default => new FireFlySettings()
    {
        habitatReference = Entity.Null,
        fireflySpeed = 1.0f,

        cycleDuration = 5f,
        cycleLitDuration = 1f,
        cycleDormantBrightness = 0.1f,
        cycleLitBrightness = 10f,
        neighborInfluenceRadius = 3f,
        neighborInfluenceDelta = 0.01f,
        fireflyDormantScale = 0.1f,
        fireflyLitScale = 0.5f,
    };
}

public struct FireFlyState : IComponentData
{
    public uint currentCellRef;
}

public struct FireFlyLightingCycle : IComponentData
{
    public float Value;
}

public struct FireFlyDestination : IComponentData
{
    public float3 Value;
}

public struct FireFlyHabitat : IComponentData
{
    public Entity fireflyPrefabEntity;
    public float3 habitatCornerA;
    public float3 habitatCornerB;
    public float fireflyMinimumSpeed;
    public float fireflyMaximumSpeed;
    public float3 cellSize;
    public uint3 partitionDimentions;
}

public struct FireFlyHabitatCellPopulation : IBufferElementData
{
    public Entity Value;
}