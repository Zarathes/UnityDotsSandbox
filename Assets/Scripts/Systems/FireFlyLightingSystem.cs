// Author: Bart Schut
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
public class FireFlyLightingSystem : SystemBase
{
    private EntityQuery m_fireFlyQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        RequireForUpdate(m_fireFlyQuery);
    }

    protected override void OnUpdate()
    {
        var fireflyStateFromEntity = GetComponentDataFromEntity<FireFlyLightingCycle>();
        var fireflySettingsFromEntity = GetComponentDataFromEntity<FireFlySettings>(true);

        var deltaTime = World.Time.DeltaTime;
        Entities
            .WithStoreEntityQueryInField(ref m_fireFlyQuery)
            .WithReadOnly(fireflySettingsFromEntity)
            .ForEach((Entity entity, ref Scale scale, ref ColorBrightness brightnessData) => 
            {
                var fireflyState = fireflyStateFromEntity[entity];
                var fireflySettings = fireflySettingsFromEntity[entity];
                var cyclePeak = fireflySettings.cycleDuration - fireflySettings.cycleLitDuration / 2.0f;
                fireflyState.Value += deltaTime;
                if (fireflyState.Value > fireflySettings.cycleDuration) // make sure phase stays within cycle
                {
                    fireflyState.Value -= fireflySettings.cycleDuration;  
                }
                if(fireflyState.Value - deltaTime < cyclePeak && fireflyState.Value > cyclePeak) // went past the peak cycle this frame
                {
                    fireflyState.Value = cyclePeak;
                }
                //if (fireflyState.cycleProgress != cyclePeak && HasComponent<HasTriggerEventsTag>(entity))
                //{
                //    var neighbors = GetBuffer<TriggerEventBufferElement>(entity);
                //    for (int index = 0; index < neighbors.Length; index++)
                //    {
                //        var neighbor = neighbors[index].Entity;
                //        if (!fireflyStateFromEntity.HasComponent(neighbor))
                //        {
                //            continue;
                //        }
                //        var neighborState = fireflyStateFromEntity[neighbor];
                //        var neighborSettings = fireflySettingsFromEntity[neighbor];
                //        var neighborCyclePeak = neighborSettings.cycleDuration - neighborSettings.cycleLitDuration / 2.0f;
                //        if (neighborState.cycleProgress != neighborCyclePeak)
                //        {
                //            continue;
                //        }
                //        if (fireflyState.cycleProgress < cyclePeak) // before peak
                //        {
                //            fireflyState.cycleProgress = math.min(cyclePeak, fireflyState.cycleProgress + neighborSettings.neighborInfluenceDelta);
                //        }
                //        else
                //        {
                //            fireflyState.cycleProgress += neighborSettings.neighborInfluenceDelta;
                //            if (fireflyState.cycleProgress > fireflySettings.cycleDuration) // make sure phase stays within cycle
                //            {
                //                fireflyState.cycleProgress -= fireflySettings.cycleDuration;
                //            }
                //        }
                //        if (fireflyState.cycleProgress == cyclePeak)
                //        {
                //            break;
                //        }
                //    }
                //}
                fireflyStateFromEntity[entity] = fireflyState;
                brightnessData.Value = computeBrightness(fireflyState, fireflySettings);
                scale.Value = RemapValue(brightnessData.Value, fireflySettings.cycleDormantBrightness, fireflySettings.cycleLitBrightness, fireflySettings.fireflyDormantScale, fireflySettings.fireflyLitScale);
            }).Schedule();
    }

    private static float computeBrightness(in FireFlyLightingCycle fireflyState, in FireFlySettings fireflySettings)
    {
        float brightness;
        // Compute own brightness
        if (fireflyState.Value < fireflySettings.cycleDuration - fireflySettings.cycleLitDuration)
        {
            brightness = fireflySettings.cycleDormantBrightness;
        }
        else if (fireflyState.Value < fireflySettings.cycleDuration - fireflySettings.cycleLitDuration / 2.0f)
        {
            brightness = fireflySettings.cycleDormantBrightness
            + (fireflySettings.cycleLitBrightness - fireflySettings.cycleDormantBrightness)
            * (fireflyState.Value - (fireflySettings.cycleDuration - fireflySettings.cycleLitDuration))
            / (fireflySettings.cycleLitDuration / 2.0f);
        }
        else
        {
            brightness = fireflySettings.cycleLitBrightness
            - (fireflySettings.cycleLitBrightness - fireflySettings.cycleDormantBrightness)
            * (fireflyState.Value - (fireflySettings.cycleDuration - fireflySettings.cycleLitDuration / 2.0f))
            / (fireflySettings.cycleLitDuration / 2.0f);
        }
        return brightness;
    }

    private static float RemapValue(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
