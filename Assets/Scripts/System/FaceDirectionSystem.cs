using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class FaceDirectionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Entities
            .WithBurst()
            .ForEach((ref Rotation rotation, in Translation translation, in MoveData movement) =>
            {
                if (movement.direction.Equals(float3.zero))
                    return;

                quaternion targetRotation = quaternion.LookRotationSafe(movement.direction, math.up());
                rotation.Value = math.slerp(rotation.Value, targetRotation, movement.turnSpeed);
            })
            .ScheduleParallel();
    }
}