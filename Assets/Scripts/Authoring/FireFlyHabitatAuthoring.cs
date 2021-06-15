// Author: Bart Schut
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class FireFlyHabitatAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject fireflyPrefab;
    public float fireflyMinimumSpeed;
    public float fireflyMaximumSpeed;

    public uint rows = 2; // X plane
    public uint collums = 2; // Y Plane
    public uint slices = 2; // Z plane

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
            fireflyMinimumSpeed = fireflyMinimumSpeed,
            fireflyMaximumSpeed = fireflyMaximumSpeed,
            habitatCornerA = position,
            habitatCornerB = new float3(position.x + scale.x, position.y + scale.y, position.z + scale.z),
            cellSize = new float3(transform.localScale.x / rows, transform.localScale.y / collums, transform.localScale.z / slices),
            partitionDimentions = new uint3(rows, collums, slices)
        });
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + transform.localScale / 2, transform.localScale);
        Gizmos.color = Color.red;
        var cellSize = new float3(transform.localScale.x / rows, transform.localScale.y / collums, transform.localScale.z / slices);
        for (uint index = 0; index < rows * collums * slices; index++)
        {
            var coordinates = HelperFunctions.GetCellCoordinates(index, new uint3(rows,collums, slices));
            var cellOrigin = new float3(transform.position.x + coordinates.x * cellSize.x, transform.position.y + coordinates.y * cellSize.y, transform.position.z + coordinates.z * cellSize.z) + cellSize / 2;
            Gizmos.DrawWireCube(cellOrigin, cellSize);
        }
    }
#endif
}
