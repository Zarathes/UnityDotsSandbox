using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(MovementSystem))]
public class SpawnerSystem : SystemBase
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer.ParallelWriter commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        float deltaTime = Time.DeltaTime;

        Entities
            .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .WithAll<SpawnerTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref IntervalSpawnerData spawner, in Translation translation, in Rotation rotation) =>
            {
                    if (spawner.cooldown >= 0.0f)
                    {
                        spawner.cooldown -= deltaTime;
                        return;
                    }
                    else
                        spawner.cooldown = spawner.interval;

                var instance = commandBuffer.Instantiate(entityInQueryIndex, spawner.spawnPrefab);
                commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation { Value = translation.Value });
                commandBuffer.SetComponent(entityInQueryIndex, instance, new Rotation { Value = rotation.Value });
            }).ScheduleParallel();

        Entities
            .WithAll<SpawnerTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref InitializeRespawnData spawner, in LocalToWorld location) =>
            {
                var instance = commandBuffer.Instantiate(entityInQueryIndex, spawner.spawnPrefab);
                commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation { Value = location.Position });
                commandBuffer.AddComponent(entityInQueryIndex, instance, new RespawnData { spawnerReference = entity });
                commandBuffer.RemoveComponent<InitializeRespawnData>(entityInQueryIndex, entity);
            }).ScheduleParallel();

        ComponentDataFromEntity<Translation> allTranslations = GetComponentDataFromEntity<Translation>(true);
        Entities
            .WithAll<RequiresRespawnTag>()
            .WithReadOnly(allTranslations)
            .ForEach((Entity entity, int entityInQueryIndex, ref RespawnData spawnData) =>
            {

                HealthData healthData = GetComponent<HealthData>(entity);
                commandBuffer.SetComponent(entityInQueryIndex, entity, new Translation { Value = allTranslations[spawnData.spawnerReference].Value });
                commandBuffer.SetComponent(entityInQueryIndex, entity, new HealthData
                {
                    maxHealthPoints = healthData.maxHealthPoints,
                    healthPoints = healthData.maxHealthPoints
                });
                commandBuffer.RemoveComponent<RequiresRespawnTag>(entityInQueryIndex, entity);
            }).ScheduleParallel();
        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}