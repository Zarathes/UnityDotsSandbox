using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class SpinnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Entities
            .WithBurst()
            .ForEach((ref Rotation rotation, in RotateData rotationData) =>
            {
                rotation.Value = math.mul(math.normalize(rotation.Value), quaternion.Euler(rotationData.rotation * rotationData.turnSpeed * deltaTime));
            })
            .ScheduleParallel();
    }
}