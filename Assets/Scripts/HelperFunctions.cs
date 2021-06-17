using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public class HelperFunctions
{
    public static NativeHashMap<uint, Entity> persistentHabitatSpacePartitioning = new NativeHashMap<uint, Entity>();
    public static uint GetCellIndexFromPosition(float3 position, float3 cellSize, uint3 coordinateBounds)
    {
        uint3 coordinates = uint3.zero;
        coordinates.x = (uint)math.clamp(math.floor(position.x / cellSize.x), 0, coordinateBounds.x);
        coordinates.y = (uint)math.clamp(math.floor(position.y / cellSize.y), 0, coordinateBounds.y);
        coordinates.z = (uint)math.clamp(math.floor(position.z / cellSize.z), 0, coordinateBounds.z);

        return GetCellIndex(coordinates, coordinateBounds);
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
}