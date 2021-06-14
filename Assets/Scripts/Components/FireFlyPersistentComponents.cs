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
        neighborInfluenceDelta = 0.01f,
        fireflyDormantScale = 0.1f,
        fireflyLitScale = 0.5f,
    };
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
    public int fireflyCount;
    public float3 habitatCornerA;
    public float3 habitatCornerB;
    public float fireflyMinimumSpeed;
    public float fireflyMaximumSpeed;
}
