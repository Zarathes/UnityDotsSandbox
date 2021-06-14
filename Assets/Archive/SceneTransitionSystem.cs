using Unity.Collections;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine;

[UpdateInGroup(typeof(SceneSystemGroup))]
public class SceneTransitionGroup : ComponentSystemGroup
{
    protected override void OnCreate()
    {
        base.OnCreate();
    }
}

[UpdateInGroup(typeof(SceneTransitionGroup))]
public class ScenePreload : SystemBase
{
    private EndInitializationEntityCommandBufferSystem m_entityCommandBufferSystem;
    private EntityQuery m_availibleSceneQuery;
    protected override void OnCreate()
    {
        m_entityCommandBufferSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
        m_availibleSceneQuery = GetEntityQuery(typeof(LevelScene.Unused));
        RequireForUpdate(m_availibleSceneQuery);
        RequireSingletonForUpdate<LevelScene.Loader.DoPreload>();        
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = m_entityCommandBufferSystem.CreateCommandBuffer();
        // Pick random scene out of available options
        NativeArray<Entity> nextSceneOptions = m_availibleSceneQuery.ToEntityArray(Allocator.Temp);
        Entity nextScene = nextSceneOptions[Random.Range(0, nextSceneOptions.Length)];
        nextSceneOptions.Dispose();
        // Start scene loading
        World.GetExistingSystem<SceneSystem>().LoadSceneAsync(GetComponent<LevelScene.Data>(nextScene).sceneGUID, new SceneSystem.LoadParameters() { AutoLoad = false, Flags = SceneLoadFlags.DisableAutoLoad });
        // Update scene state
        ecb.RemoveComponent<LevelScene.Unused>(nextScene);
        ecb.AddComponent<LevelScene.Initialized>(nextScene);    
        // Consume signal component
        Entity signalEntity = GetSingletonEntity<LevelScene.Loader.DoPreload>();
        ecb.RemoveComponent<LevelScene.Loader.DoPreload>(signalEntity);
        m_entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

[UpdateInGroup(typeof(SceneTransitionGroup))]
public class SceneTransition : SystemBase
{
    private EndInitializationEntityCommandBufferSystem m_entityCommandBufferSystem;
    private EntityQuery m_sceneProxyQuery;
    protected override void OnCreate()
    {
        m_entityCommandBufferSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
        m_sceneProxyQuery = GetEntityQuery(typeof(SceneReference));
        RequireSingletonForUpdate<LevelScene.Initialized>();
        RequireSingletonForUpdate<LevelScene.Loader.DoTransition>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = m_entityCommandBufferSystem.CreateCommandBuffer();
        if (HasSingleton<LevelScene.Active>())
        {
            Entity currentScene = GetSingletonEntity<LevelScene.Active>();
            ecb.RemoveComponent<LevelScene.Active>(currentScene);
            ecb.AddComponent<LevelScene.InTransition>(currentScene);
        }
        // Transition scene to proper world
        Entity nextScene = GetSingletonEntity<LevelScene.Initialized>();
        var proxies = m_sceneProxyQuery.ToEntityArray(Allocator.Temp);
        for(int index = 0; index < proxies.Length; index++)
        {
            Entity sceneEntity = proxies[index];
            if (GetComponent<SceneReference>(sceneEntity).SceneGUID == GetComponent<LevelScene.Data>(nextScene).sceneGUID)
            {
                DynamicBuffer<ResolvedSectionEntity> buffer = GetBufferFromEntity<ResolvedSectionEntity>(true)[sceneEntity];
                foreach(ResolvedSectionEntity rse in buffer)
                {
                    ecb.AddComponent(rse.SectionEntity, new RequestSceneLoaded() { LoadFlags = SceneLoadFlags.LoadAdditive });
                }
                SetComponent(sceneEntity, new RequestSceneLoaded() { LoadFlags = SceneLoadFlags.LoadAdditive });
                break;
            }
        }
        proxies.Dispose();
        // Update scene state
        ecb.RemoveComponent<LevelScene.Initialized>(nextScene);
        ecb.AddComponent<LevelScene.Active>(nextScene);
        // Consume signal component
        Entity signalEntity = GetSingletonEntity<LevelScene.Loader.DoTransition>();
        ecb.RemoveComponent<LevelScene.Loader.DoTransition>(signalEntity);
        m_entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

[UpdateInGroup(typeof(SceneTransitionGroup))]
public class SceneCleanup : SystemBase
{
    private EndInitializationEntityCommandBufferSystem m_entityCommandBufferSystem;
    protected override void OnCreate()
    {
        m_entityCommandBufferSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
        RequireSingletonForUpdate<LevelScene.InTransition>();
        RequireSingletonForUpdate<LevelScene.Loader.DoCleanup>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = m_entityCommandBufferSystem.CreateCommandBuffer();
        // Cleanup scene component
        Entity cleanupScene = GetSingletonEntity<LevelScene.InTransition>();
        ecb.RemoveComponent<LevelScene.InTransition>(cleanupScene);
        ecb.AddComponent<LevelScene.Unused>(cleanupScene);
        // Unload scene and remove entity
        LevelScene.Data data = GetComponent<LevelScene.Data>(cleanupScene);
        World.GetExistingSystem<SceneSystem>().UnloadScene(data.sceneGUID,SceneSystem.UnloadParameters.DestroySceneProxyEntity | SceneSystem.UnloadParameters.DestroySectionProxyEntities);
        // Consume signal component
        Entity signalEntity = GetSingletonEntity<LevelScene.Loader.DoCleanup>();
        ecb.RemoveComponent<LevelScene.Loader.DoCleanup>(signalEntity);
        m_entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

[UpdateInGroup(typeof(SceneTransitionGroup))]
public class SceneLoad : SystemBase
{
    private EndInitializationEntityCommandBufferSystem m_entityCommandBufferSystem;
    protected override void OnCreate()
    {
        m_entityCommandBufferSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
        RequireSingletonForUpdate<LevelScene.Loader.DoLoad>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = m_entityCommandBufferSystem.CreateCommandBuffer();
        World.GetExistingSystem<SceneSystem>().LoadSceneAsync(new Unity.Entities.Hash128("82261f90dbc6ddc49a16b0b53e9995f8"));
        // Consume signal component
        Entity signalEntity = GetSingletonEntity<LevelScene.Loader.DoLoad>();
        ecb.RemoveComponent<LevelScene.Loader.DoLoad>(signalEntity);
        m_entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

[UpdateInGroup(typeof(SceneTransitionGroup))]
public class SceneUnload : SystemBase
{
    private EndInitializationEntityCommandBufferSystem m_entityCommandBufferSystem;
    protected override void OnCreate()
    {
        m_entityCommandBufferSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
        RequireSingletonForUpdate<LevelScene.Loader.DoUnload>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = m_entityCommandBufferSystem.CreateCommandBuffer();
        World.GetExistingSystem<SceneSystem>().UnloadScene(new Unity.Entities.Hash128("82261f90dbc6ddc49a16b0b53e9995f8"));
        // Consume signal component
        Entity signalEntity = GetSingletonEntity<LevelScene.Loader.DoUnload>();
        ecb.RemoveComponent<LevelScene.Loader.DoUnload>(signalEntity);
        m_entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}