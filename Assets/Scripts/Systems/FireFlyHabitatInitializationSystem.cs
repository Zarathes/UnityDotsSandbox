// Author: Bart Schut
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class FireFlyHabitatInitializationSystem : SystemBase
{
    private EndInitializationEntityCommandBufferSystem m_barrier;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_barrier = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer commandBuffer = m_barrier.CreateCommandBuffer();
        var randomArray = World.GetExistingSystem<RandomSystem>().RandomArray;

        Entities
            .ForEach((Entity entity, in FireFlyHabitat fireFlyHabitat) =>
            {
                var random = randomArray[0];
                for (int i = 0; i < fireFlyHabitat.fireflyCount; i++)
                {
                    var fireflySettings = GetComponent<FireFlySettings>(fireFlyHabitat.fireflyPrefabEntity);
                    fireflySettings.fireflySpeed = random.NextFloat(fireFlyHabitat.fireflyMinimumSpeed, fireFlyHabitat.fireflyMaximumSpeed);
                    fireflySettings.habitatReference = entity;
                    var spawnPosition = random.NextFloat3(fireFlyHabitat.habitatCornerA, fireFlyHabitat.habitatCornerB);
                    var fireFlyEntity = commandBuffer.Instantiate(fireFlyHabitat.fireflyPrefabEntity);
                    commandBuffer.SetComponent(fireFlyEntity, new FireFlyLightingCycle() { Value = random.NextFloat(0.0f, fireflySettings.cycleDuration) });
                    commandBuffer.SetComponent(fireFlyEntity, new Translation() { Value = spawnPosition });
                    commandBuffer.SetComponent(fireFlyEntity, fireflySettings);
                }
                randomArray[0] = random;
            })
            .WithName("FireFlySpawnJob")
            .Schedule();
        m_barrier.AddJobHandleForProducer(Dependency);
        Enabled = false;
    }
}