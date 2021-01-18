using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

public class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities
            .WithBurst()
            .WithAny<BulletTag, TrainTag, PlayerTag>()
            .ForEach((ref Translation translation, in MoveData movement) =>
            {
                float3 normalizedDir = math.normalizesafe(movement.direction);
                translation.Value += normalizedDir * movement.movementSpeed * deltaTime;

            })
            .ScheduleParallel();

        //Entities
        //.WithBurst()
        //.WithAny()
        //.ForEach((ref PhysicsVelocity translation, in MoveData movement) =>
        //{
        //    float3 normalizedDir = math.normalizesafe(movement.direction);
        //    translation.Linear += normalizedDir * movement.movementSpeed * deltaTime;

        //})
        //.ScheduleParallel();
    }
}