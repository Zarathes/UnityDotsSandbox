// Author: Bart Schut
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
public class FireFlyMovementSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem m_barrier;
    private EntityQuery movingFireFlyQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_barrier = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        RequireForUpdate(movingFireFlyQuery);
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer.ParallelWriter commandBuffer = m_barrier.CreateCommandBuffer().AsParallelWriter();
        var deltaTime = World.Time.DeltaTime;
        Entities
            .WithStoreEntityQueryInField(ref movingFireFlyQuery)
            .ForEach((Entity entity, int entityInQueryIndex, ref Translation position, in FireFlySettings fireFlySettings, in FireFlyDestination destination) =>
            {
                position.Value += math.normalize(destination.Value - position.Value) * fireFlySettings.fireflySpeed * deltaTime;
                if (math.length(destination.Value - position.Value) < 1.0f)
                {
                    commandBuffer.RemoveComponent<FireFlyDestination>(entityInQueryIndex, entity);
                }
            })
            .WithName("FireFlyMovementJob")
            .ScheduleParallel();
        m_barrier.AddJobHandleForProducer(Dependency);
    }
}
