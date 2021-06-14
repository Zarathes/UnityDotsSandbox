// Author: Bart Schut
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class FireFlyHabitatAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject fireflyPrefab;
    public int fireflySpawnAmount;
    public float fireflyMinimumSpeed;
    public float fireflyMaximumSpeed;
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(fireflyPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var position = transform.position;
        var scale = transform.localScale;
        dstManager.AddComponentData(entity, new FireFlyHabitat() 
        { 
            fireflyPrefabEntity = conversionSystem.GetPrimaryEntity(fireflyPrefab),
            fireflyCount = fireflySpawnAmount,
            fireflyMinimumSpeed = fireflyMinimumSpeed,
            fireflyMaximumSpeed = fireflyMaximumSpeed,
            habitatCornerA = new float3(position.x - scale.x / 2.0f, position.y - scale.y / 2.0f, position.z - scale.z / 2.0f),
            habitatCornerB = new float3(position.x + scale.x / 2.0f, position.y + scale.y / 2.0f, position.z + scale.z / 2.0f),
        });
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
#endif
}
