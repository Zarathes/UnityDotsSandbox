using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using System.Linq;

public class TargetingSystem : SystemBase
{
    private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

    private EntityQuery _playerQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        _playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerTag>());
        _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        var playerEntityArray = _playerQuery.ToEntityArray(Allocator.Temp);
        var playerEntity = (playerEntityArray.Count() != 0) ? playerEntityArray.First() : Entity.Null;
        Entities
            .WithBurst()
            .ForEach((Entity entity, int entityInQueryIndex, ref MoveData movement, in Translation translation, in Rotation rotation, in TargetData target) =>
            {
                if (target.isSmartBullet && playerEntity != Entity.Null)
                {
                    Translation playerTranslation = GetComponent<Translation>(playerEntity);
                    movement.direction = math.normalizesafe(playerTranslation.Value - translation.Value);
                }
                else
                {
                    movement.direction = math.mul(rotation.Value, new float3(0.0f, 0.0f, 1.0f));
                }
                commandBuffer.RemoveComponent<TargetData>(entityInQueryIndex, entity);
            })
            .ScheduleParallel();

        playerEntityArray.Dispose();
        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}