// Author: Bart Schut
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;

/// <summary>Handles initialization, storage, and disposal of
/// thread-indexed random number generators of the
/// Unity.Mathematics.Random variety.</summary>
[UpdateInGroup(typeof(InitializationSystemGroup))]
public class RandomSystem : ComponentSystem
{
    /// <summary>A NativeArray of thread-indexed Unity.Mathematics.Random
    /// number generators.</summary>
    public NativeArray<Random> RandomArray { get; private set; }

    protected override void OnCreate()
    {
        var randomArray = new Random[JobsUtility.MaxJobThreadCount];
        var seed = new System.Random();

        for (var index = 0; index < JobsUtility.MaxJobThreadCount; ++index)
        {
            randomArray[index] = new Random((uint)seed.Next());
        }

        RandomArray = new NativeArray<Random>(randomArray, Allocator.Persistent);
        Enabled = false;
    }

    protected override void OnDestroy()
    { 
        RandomArray.Dispose();
    }

    protected override void OnUpdate() 
    {
    }
}
