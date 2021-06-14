using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;


public class UserInputSystem : SystemBase
{
    protected override void OnUpdate()
    {

        // Loop over all entities with input and movement component
        Entities
            .WithBurst()
            .WithAll<PlayerTag>()
            .ForEach((ref MoveData movement, in UserInputData input) =>
            {
                float3 newDirection = new float3(0.0f, 0.0f, 0.0f);
                newDirection.z += Input.GetKey(input.upAction) ? 1 : 0;
                newDirection.z -= Input.GetKey(input.downAction) ? 1 : 0;
                newDirection.x += Input.GetKey(input.rightAction) ? 1 : 0;
                newDirection.x -= Input.GetKey(input.leftAction) ? 1 : 0;
                movement.direction = newDirection;
            })
            .Run();
    }
}