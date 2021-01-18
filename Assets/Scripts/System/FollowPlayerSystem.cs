using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;
using Unity.Collections;
using System.Linq;

[UpdateAfter(typeof(SpawnerSystem))]

public class FollowPlayerSystem : SystemBase
{
    private EntityQuery _playerQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        _playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerTag>());
    }

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        var playerEntityArray = _playerQuery.ToEntityArray(Allocator.Temp);
        var playerEntity = (playerEntityArray.Count() != 0) ? playerEntityArray.First() : Entity.Null;
        ComponentDataFromEntity<Translation> allTranslations = GetComponentDataFromEntity<Translation>(false);
        ComponentDataFromEntity<Rotation> AllRotations = GetComponentDataFromEntity<Rotation>(false);

        Entities
            .WithoutBurst()
            .WithAll<CameraTag, Translation>()
            .ForEach((Entity entity, Camera camera) =>
            {
                if (playerEntity != Entity.Null)
                {
                    Translation playerTranslation = allTranslations[playerEntity];
                    Translation cameraTranslation = allTranslations[entity];
                    cameraTranslation.Value.x = playerTranslation.Value.x;
                    cameraTranslation.Value.z = playerTranslation.Value.z;
                    allTranslations[entity] = cameraTranslation;
                    camera.transform.position = cameraTranslation.Value;

                    //Rotation playerRotation = AllRotations[playerEntity];
                    //playerRotation.Value.value.
                    //Rotation cameraRotation = AllRotations[entity];
                    //cameraRotation.Value = math.add(cameraRotation.Value, playerRotation.Value);
                    //camera.transform.rotation = cameraRotation.Value;
                }
            })
            .Run();
        playerEntityArray.Dispose();
    }
}