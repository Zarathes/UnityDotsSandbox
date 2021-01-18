using System.Collections.Generic;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

public class GameModeAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameModeData m_GameMode;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(gameObject);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        SceneSection ss = dstManager.GetSharedComponentData<SceneSection>(entity);
        for (int index = 0; index < m_GameMode.SceneList.Count; index++)
        {
            GameModeData.SceneInfo scene = m_GameMode.SceneList[index];
            Entity sceneEntity = dstManager.CreateEntity(typeof(LevelScene.Unused));
            dstManager.AddSharedComponentData(sceneEntity, ss);
            string guid;
            long file;
            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(scene.m_Scene, out guid, out file))
            {
                dstManager.AddComponentData(sceneEntity, new LevelScene.Data { sceneDefinition = scene.sceneDefinition, sceneGUID = new Unity.Entities.Hash128(guid) });
            }
        }
    }
}
