using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Stateful;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(ExportPhysicsWorld))]
[UpdateAfter(typeof(TriggerEventConversionSystem))]
public class DeathOnCollisionSystem : SystemBase
{
    private ExportPhysicsWorld exportPhysicsWorld;
    private EndFramePhysicsSystem endFramePhysicsSystem;
    private TriggerEventConversionSystem triggerSystem;

    private EntityQueryMask hierarchyChildMask;
    private EntityQueryMask triggerDynamicBodyMask;

    protected override void OnCreate()
    {
        base.OnCreate();
        exportPhysicsWorld = World.GetOrCreateSystem<ExportPhysicsWorld>();
        endFramePhysicsSystem = World.GetOrCreateSystem<EndFramePhysicsSystem>();
        triggerSystem = World.GetOrCreateSystem<TriggerEventConversionSystem>();

        hierarchyChildMask = EntityManager.GetEntityQueryMask(
            GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(Parent),
                    typeof(LocalToWorld)
                }
            })
        );
       triggerDynamicBodyMask = EntityManager.GetEntityQueryMask(
            GetEntityQuery(new EntityQueryDesc
            {
                Any = new ComponentType[]
                {
                    typeof(PlayerTag),
                    typeof(BulletTag)
                },
                All = new ComponentType[]
                {
                    typeof(HealthData)
                }
            })
        );
    }

    protected override void OnUpdate()
    {
        Dependency = JobHandle.CombineDependencies(exportPhysicsWorld.GetOutputDependency(), Dependency);
        var deltaTime = Time.DeltaTime;

        // Need extra variables here so that they can be
        // captured by the Entities.Foreach loop below
        var localHierarchyChildMask = hierarchyChildMask;
        var localTriggerDynamicBodyMask = triggerDynamicBodyMask;

        //Entities
        //    .WithBurst()
        //    .WithAll<BulletTag>()
        //    .ForEach((Entity bullet, ref DynamicBuffer<StatefulTriggerEvent> triggerBuffer, ref HealthData health, in DamageData damage) =>
        //    {
        //        for (int i = 0; i < triggerBuffer.Length; i++)
        //        {
        //            var triggerEvent = triggerBuffer[i];
        //            var otherEntity = triggerEvent.GetOtherEntity(bullet);

        //            // Check if trigger is with a bullet
        //            if (triggerEvent.State == EventOverlapState.Enter && localTriggerDynamicBodyMask.Matches(otherEntity))
        //            {
        //                if(HasComponent<HealthData>(otherEntity))
        //                {
        //                    // Kill damage collider
        //                    HealthData healthData = GetComponent<HealthData>(otherEntity);
        //                    healthData.healthPoints -= damage.damagePoints;
        //                    SetComponent(otherEntity, healthData);
        //                    health.healthPoints--;
        //                }
        //            }             
        //        }
        //    }).Schedule();
        endFramePhysicsSystem.AddInputDependency(Dependency);
    }            
}
