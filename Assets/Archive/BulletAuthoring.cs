//using System.Collections.Generic;
//using Unity.Entities;
//using UnityEngine;

//[RequiresEntityConversion]
//public class BulletAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
//{
//    public float maxLifeTime;

//    public bool isSmartBullet;
//    public float bulletSpeed;

//    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
//    {
//        referencedPrefabs.Add(gameObject);
//    }

//    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
//    {
//#if UNITY_EDITOR
//        dstManager.SetName(entity, gameObject.name);
//#endif
//        dstManager.AddComponentData(entity, new LifeTimeData
//        {
//            maxLifeTime = maxLifeTime,
//            creationTime = double.MinValue
//        });
//        dstManager.AddComponentData(entity, new MoveData
//        {
//            movementSpeed = bulletSpeed
//        });
//        dstManager.AddComponentData(entity, new SetupTargetData
//        {
//            isSmartBullet = isSmartBullet
//        });
//    }
//}
