using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class HelperFunctions
{
    // This is initiated and filled in MapEraDataSystemClient.
    public static NativeHashMap<uint, Entity> persistentHabitatSpacePartitioning = new NativeHashMap<uint, Entity>();
    public static uint GetCellIndexFromPosition(float3 position, float3 cellSize, uint3 coordinateBounds)
    {
        uint3 coordinates = uint3.zero;
        coordinates.x = (uint)math.clamp(math.floor(position.x / cellSize.x), 0, coordinateBounds.x);
        coordinates.y = (uint)math.clamp(math.floor(position.y / cellSize.y), 0, coordinateBounds.y);
        coordinates.z = (uint)math.clamp(math.floor(position.z / cellSize.z), 0, coordinateBounds.z);

        var cellIndex = GetCellIndex(coordinates, coordinateBounds);
        var cellRef = GetCellCoordinates(cellIndex, coordinateBounds);
        return cellIndex;
    }

    public static uint GetCellIndex(uint3 coordinates, uint3 coordinateBounds)
    {
        return coordinates.z * coordinateBounds.x * coordinateBounds.y + coordinates.y * coordinateBounds.x + coordinates.x;
    }

    public static uint3 GetCellCoordinates(uint index, uint3 coordinateBounds)
    {
        uint3 result;
        uint a = (coordinateBounds.x * coordinateBounds.y);
        result.z = index / a;
        uint b = index - a * result.z;
        result.y = b / coordinateBounds.x;
        result.x = b % coordinateBounds.x;
        return result;
    }

    public static NativeList<uint> GetNeighborsIncludingSelf(uint cellRef, uint3 coordinateBounds)
    {
        NativeList<uint> neighbours = new NativeList<uint>(Allocator.Temp);
        neighbours.Add(cellRef);
        var coordinates = GetCellCoordinates(cellRef, coordinateBounds);
        for (int index1 = -1; index1 <= 1; ++index1)
        {
            for (int index2 = -1; index2 <= 1; ++index2)
            {
                for (int index3 = -1; index3 <= 1; ++index3)
                {
                    int x1 = (int)(coordinates.x + index1);
                    int x2 = (int)(coordinates.y + index2);
                    int x3 = (int)(coordinates.z + index3);
                    if (x1 >= 0 && x1 < coordinateBounds.x
                        && x2 >= 0 && x2 < coordinateBounds.y
                        && x3 >= 0 && x3 < coordinateBounds.z)
                    {
                        neighbours.Add((uint)((x1 * coordinateBounds.y + x2) * coordinateBounds.z + x3));
                    }
                }
            }
        }
        return neighbours;
    }
}