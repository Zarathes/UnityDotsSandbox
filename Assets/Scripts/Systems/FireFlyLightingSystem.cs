// Author: Bart Schut
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(FireFlyCycleSystem))]
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
        Entities
            .WithStoreEntityQueryInField(ref m_fireFlyQuery)
            .ForEach((ref Scale scale, ref ColorBrightness brightnessData, in FireFlySettings settings, in FireFlyLightingCycle cycle) => 
            {
                // Compute own brightness
                float brightness;
                if (cycle.Value < settings.cycleDuration - settings.cycleLitDuration)
                {
                    brightness = settings.cycleDormantBrightness;
                }
                else if (cycle.Value < settings.cycleDuration - settings.cycleLitDuration / 2.0f)
                {
                    brightness = settings.cycleDormantBrightness
                    + (settings.cycleLitBrightness - settings.cycleDormantBrightness)
                    * (cycle.Value - (settings.cycleDuration - settings.cycleLitDuration))
                    / (settings.cycleLitDuration / 2.0f);
                }
                else
                {
                    brightness = settings.cycleLitBrightness
                    - (settings.cycleLitBrightness - settings.cycleDormantBrightness)
                    * (cycle.Value - (settings.cycleDuration - settings.cycleLitDuration / 2.0f))
                    / (settings.cycleLitDuration / 2.0f);
                }
                brightnessData.Value = brightness;
                var remap = (brightness - 0.8f) / (settings.cycleLitBrightness - 0.8f) * (settings.fireflyLitScale - settings.fireflyDormantScale) + settings.fireflyDormantScale;
                scale.Value = Mathf.Max(0.0f, remap);
            })
            .WithName("FireFlyLightingJob")
            .ScheduleParallel();
    }
}
