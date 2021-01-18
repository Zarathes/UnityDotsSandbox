using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[UpdateBefore(typeof(SpawnerSystem))]
public class LifeTimeSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem commandBufferSystem;

    protected override void OnCreate()
    {
        commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer.ParallelWriter commandBuffer = commandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        float deltaTime = Time.DeltaTime;

        Entities
            .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .WithAny<BulletTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref LifeTimeData lifeTime, in HealthData health) =>
            {
                lifeTime.lifeTime -= deltaTime;
                if (lifeTime.lifeTime <= 0 || health.healthPoints <= 0)
                    commandBuffer.DestroyEntity(entityInQueryIndex, entity);
            }).ScheduleParallel();

        Entities
            .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .WithAny<TrainTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref LifeTimeData lifeTime) =>
            {
                lifeTime.lifeTime -= deltaTime;
                if (lifeTime.lifeTime <= 0)
                    commandBuffer.DestroyEntity(entityInQueryIndex, entity);
            }).ScheduleParallel();

        Entities
            .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .WithAny<PlayerTag>()
            .ForEach((Entity entity, int entityInQueryIndex, in HealthData health) =>
            {
                if (health.healthPoints <= 0)
                    commandBuffer.AddComponent<RequiresRespawnTag>(entityInQueryIndex, entity);
            }).ScheduleParallel();

        commandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}