// Author: Bart Schut
using Unity.Collections;
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
        var habitat = GetSingleton<FireFlyHabitat>();
        if(!HelperFunctions.persistentHabitatSpacePartitioning.IsCreated)
        {
            var totalSize = habitat.partitionDimentions.x * habitat.partitionDimentions.y * habitat.partitionDimentions.z;
            HelperFunctions.persistentHabitatSpacePartitioning = new NativeHashMap<uint, Entity>((int)totalSize, Allocator.Persistent);
            for (uint index = 0; index < totalSize; index++)
            {
                var cellEntity = EntityManager.CreateEntity();
                EntityManager.AddBuffer<FireFlyHabitatCellPopulation>(cellEntity);
                HelperFunctions.persistentHabitatSpacePartitioning.Add(index, cellEntity);
            }
        }

        EntityCommandBuffer commandBuffer = m_barrier.CreateCommandBuffer();
        var habitatEntity = GetSingletonEntity<FireFlyHabitat>();
        var positionOffset = GetComponent<Translation>(habitatEntity).Value;
        var habitatSpace = HelperFunctions.persistentHabitatSpacePartitioning;
         var deltaTime = World.Time.DeltaTime;
        Entities
            .WithStoreEntityQueryInField(ref movingFireFlyQuery)
            .WithReadOnly(habitatSpace)
            .ForEach((Entity entity, ref Translation position, ref FireFlyState state, in FireFlySettings fireFlySettings, in FireFlyDestination destination) =>
            {
                position.Value += math.normalize(destination.Value - position.Value) * fireFlySettings.fireflySpeed * deltaTime;
                var newCellRef = HelperFunctions.GetCellIndexFromPosition(position.Value - positionOffset, habitat.cellSize, habitat.partitionDimentions);
                if (state.currentCellRef != newCellRef)
                {
                    var removeCellBuffer = commandBuffer.SetBuffer<FireFlyHabitatCellPopulation>(habitatSpace[state.currentCellRef]).Reinterpret<Entity>();
                    for(int index =0; index < removeCellBuffer.Length; index++)
                    { 
                        if(removeCellBuffer[index] == entity)
                        {
                            removeCellBuffer.RemoveAt(index);
                            break;
                        }
                    }                    
                    var addCellBuffer = commandBuffer.SetBuffer<FireFlyHabitatCellPopulation>(habitatSpace[newCellRef]);
                    addCellBuffer.Add(new FireFlyHabitatCellPopulation()
                    {
                        Value = entity
                    });
                    state.currentCellRef = newCellRef;
                }
                if (math.length(destination.Value - position.Value) < 1.0f)
                {
                    commandBuffer.RemoveComponent<FireFlyDestination>(entity);
                }
            })
            .WithName("FireFlyMovementJob")
            .Schedule();
        m_barrier.AddJobHandleForProducer(Dependency);
    }
}
