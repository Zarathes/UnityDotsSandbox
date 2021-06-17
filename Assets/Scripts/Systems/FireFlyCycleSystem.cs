// Author: Bart Schut
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
public class FireFlyCycleSystem : SystemBase
{
    private EntityQuery m_fireFlyQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        RequireForUpdate(m_fireFlyQuery);
    }

    protected override void OnUpdate()
    {
        if(!HelperFunctions.persistentHabitatSpacePartitioning.IsCreated)
        {
            return;
        }               
        var colorBrightnessFromEntity = GetComponentDataFromEntity<ColorBrightness>(true);
        var fireFlySettingsFromEntity = GetComponentDataFromEntity<FireFlySettings>(true);
        var translationFromEntity = GetComponentDataFromEntity<Translation>(true);

        var habitatSpace = HelperFunctions.persistentHabitatSpacePartitioning;
        var habitatCellPopulationBufferFromEntity = GetBufferFromEntity<FireFlyHabitatCellPopulation>(true); 
        var habitat = GetSingleton<FireFlyHabitat>();

        var deltaTime = World.Time.DeltaTime;
        Entities
            .WithStoreEntityQueryInField(ref m_fireFlyQuery)
            .WithReadOnly(colorBrightnessFromEntity)
            .WithReadOnly(fireFlySettingsFromEntity)
            .WithReadOnly(translationFromEntity)
            .WithReadOnly(habitatSpace)
            .WithReadOnly(habitatCellPopulationBufferFromEntity)
            .ForEach((Entity entity, ref FireFlyLightingCycle fireflyLightingCycle, in FireFlyState state) =>
            {
                var fireflySettings = fireFlySettingsFromEntity[entity];
                var cyclePeak = fireflySettings.cycleDuration - fireflySettings.cycleLitDuration / 2.0f;
                fireflyLightingCycle.Value += deltaTime;
                if (fireflyLightingCycle.Value > fireflySettings.cycleDuration) // make sure phase stays within cycle
                {
                    fireflyLightingCycle.Value -= fireflySettings.cycleDuration;
                }
                if (fireflyLightingCycle.Value - deltaTime < cyclePeak && fireflyLightingCycle.Value > cyclePeak) // went past the peak cycle this frame
                {
                    fireflyLightingCycle.Value = cyclePeak;
                    return;
                }

                var coordinates = HelperFunctions.GetCellCoordinates(state.currentCellRef, habitat.partitionDimentions);
                for (int index1 = -1; index1 <= 1; ++index1)
                {
                    for (int index2 = -1; index2 <= 1; ++index2)
                    {
                        for (int index3 = -1; index3 <= 1; ++index3)
                        {
                            int x1 = (int)(coordinates.x + index1);
                            int x2 = (int)(coordinates.y + index2);
                            int x3 = (int)(coordinates.z + index3);
                            if (x1 >= 0 && x1 < habitat.partitionDimentions.x
                                && x2 >= 0 && x2 < habitat.partitionDimentions.y
                                && x3 >= 0 && x3 < habitat.partitionDimentions.z)
                            {
                                var cellRef = HelperFunctions.GetCellIndex(new uint3((uint)x1, (uint)x2, (uint)x3), habitat.partitionDimentions);
                                var cellEntity = habitatSpace[cellRef];
                                var neighbors = habitatCellPopulationBufferFromEntity[cellEntity];
                                for (int index = 0; index < neighbors.Length; index++)
                                {
                                    var neighbor = neighbors[index].Value;
                                    if (neighbor == entity) // skip self
                                    {
                                        continue;
                                    }
                                    var positionSelf = translationFromEntity[entity];
                                    var positionOther = translationFromEntity[neighbor];
                                    if (math.length(positionOther.Value - positionSelf.Value) > fireflySettings.neighborInfluenceRadius) // Skip out of bounds
                                    {
                                        continue;
                                    }
                                    var neighborColorBrightness = colorBrightnessFromEntity[neighbor];
                                    var neighborSettings = fireFlySettingsFromEntity[neighbor];
                                    var neighborCyclePeak = neighborSettings.cycleLitBrightness == neighborColorBrightness.Value;
                                    if (!neighborCyclePeak)
                                    {
                                        continue;
                                    }
                                    if (fireflyLightingCycle.Value < cyclePeak) // before peak
                                    {
                                        fireflyLightingCycle.Value = math.min(cyclePeak, fireflyLightingCycle.Value + neighborSettings.neighborInfluenceDelta);
                                    }
                                    else
                                    {
                                        fireflyLightingCycle.Value += neighborSettings.neighborInfluenceDelta;
                                        if (fireflyLightingCycle.Value > fireflySettings.cycleDuration) // make sure phase stays within cycle
                                        {
                                            fireflyLightingCycle.Value -= fireflySettings.cycleDuration;
                                        }
                                    }
                                    if (fireflyLightingCycle.Value == cyclePeak)
                                    {
                                        break;
                                    }
                                }

                            }
                        }
                    }
                }
            })
            .WithName("FireFlyCycleJob")
            .ScheduleParallel();
    }
}
