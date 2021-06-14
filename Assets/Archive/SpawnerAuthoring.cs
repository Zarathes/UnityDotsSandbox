//using System.Collections.Generic;
//using Unity.Entities;
//using Unity.Mathematics;
//using UnityEngine;

//[RequiresEntityConversion]
//public class SpawnerAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
//{
//    public bool destroyAfterUse;
//    public float spawnCooldown;
//    public float3 rotation;
//    public GameObject spawnPrefab;

//    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
//    {
//        referencedPrefabs.Add(spawnPrefab);
//    }

//    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
//    {
//#if UNITY_EDITOR
//        dstManager.SetName(entity, spawnPrefab.name + "Spawner");
//#endif
//        dstManager.AddComponentData(entity, new SpawnData
//        {
//            spawnPrefab = conversionSystem.GetPrimaryEntity(spawnPrefab),
//            destroyAfterUse = destroyAfterUse,
//            cooldown = spawnCooldown
//        });

//        if(!rotation.Equals(float3.zero))
//        {
//            dstManager.AddComponentData(entity, new RotateData
//            {
//                rotation = rotation
//            });
//        }
//    }
//}
