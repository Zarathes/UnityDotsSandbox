// Author: Bart Schut
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class FireFlyDestinationSystem : SystemBase
{
    private EndInitializationEntityCommandBufferSystem barrier;
    private EntityQuery m_fireFlyQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        barrier = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
        RequireForUpdate(m_fireFlyQuery);
    }

    protected override void OnUpdate()
    {
        var commandBuffer = barrier.CreateCommandBuffer().AsParallelWriter();
        var fireFlyHabitatFromEntity = GetComponentDataFromEntity<FireFlyHabitat>(true);
        var randomArray = World.GetExistingSystem<RandomSystem>().RandomArray;

        Entities
            .WithStoreEntityQueryInField(ref m_fireFlyQuery)
            .WithReadOnly(fireFlyHabitatFromEntity)
            .WithNativeDisableParallelForRestriction(randomArray)
            .WithNone<FireFlyDestination>()
            .ForEach((Entity fireFlyEntity, int entityInQueryIndex, int nativeThreadIndex, in FireFlySettings settings) =>
            {
                var random = randomArray[nativeThreadIndex];
                var fireFlyHabitat = fireFlyHabitatFromEntity[settings.habitatReference];
                var newDestination = random.NextFloat3(fireFlyHabitat.habitatCornerA, fireFlyHabitat.habitatCornerB);
                commandBuffer.AddComponent(entityInQueryIndex, fireFlyEntity, new FireFlyDestination() { Value = newDestination });
                randomArray[nativeThreadIndex] = random;
            })
            .WithName("FireFlyDestinationJob")
            .ScheduleParallel();
        barrier.AddJobHandleForProducer(Dependency);
    }
}
