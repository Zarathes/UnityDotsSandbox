//using System.Collections.Generic;
//using Unity.Entities;
//using Unity.Mathematics;
//using UnityEngine;

//[RequiresEntityConversion]
//public class PlayerAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
//{
//    public float UnitSpeed;
//    [Range(0.0f, 1.0f)]
//    public float UnitTurnRate;

//    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
//    {
//        referencedPrefabs.Add(gameObject);
//    }

//    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
//    {
//#if UNITY_EDITOR
//        dstManager.SetName(entity, gameObject.name);
//#endif
//        dstManager.AddComponentData(entity, new UserInputData 
//        {
//            upAction    = KeyCode.W,    
//            downAction  = KeyCode.S,  
//            rightAction = KeyCode.A,            
//            leftAction  = KeyCode.D,  
//            jumpAction  = KeyCode.Space               
//        });
//        dstManager.AddComponentData(entity, new MoveData 
//        { 
//            movementSpeed = UnitSpeed,
//            turnSpeed = UnitTurnRate,
//        });
//        dstManager.AddComponentData(entity, new TargetData());
//    }
//}
