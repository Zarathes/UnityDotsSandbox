// Author: Bart Schut
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class FireFlyAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [Header("Lighting Behaviour")]
    [Tooltip("Full duration of one cycle, in seconds")]
    public float cycleDuration = 5.0f;
    [Tooltip("Duration of the lit period, in seconds")]
    public float cycleLitDuration = 1.0f;
    [Tooltip("Brightness when dormant")]
    public float cycleDormantBrightness = 0.1f;
    [Tooltip("Brightness when lit")]
    public float cycleLitBrightness = 10f;

    [Tooltip("Radius that fireflies influence")]
    public float synchroniseRadius = 3f;
    public float neighborInfluenceDelta = 0.01f;

    [Tooltip("Scale when dormant")]
    public float fireflyDormantScale = 0.01f;
    [Tooltip("Scale when lit")]
    public float fireflyLitScale = 0.03f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Scale() { Value = 0.05f});
        dstManager.AddComponentData(entity, new FireFlyLightingCycle());
        dstManager.AddComponentData(entity, new FireFlySettings() 
        {   
            cycleDuration = cycleDuration,
            cycleLitDuration = cycleLitDuration,
            cycleDormantBrightness = cycleDormantBrightness,
            cycleLitBrightness = cycleLitBrightness,
            neighborInfluenceDelta = neighborInfluenceDelta,
            fireflyDormantScale = fireflyDormantScale,
            fireflyLitScale = fireflyLitScale
        });
        dstManager.AddComponentData(entity, ColorBrightness.Default);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, synchroniseRadius);
    }
#endif
}
