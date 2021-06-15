// Author: Bart Schut
using UnityEngine;
using UnityEngine.UI;
using Unity.Transforms;
using Unity.Entities;
using Unity.Collections;

class FireFlySpawner : MonoBehaviour
{
    [SerializeField]
    Button Button = default;

    [SerializeField]
    Text SpawnText = default;

    [SerializeField]
    Text TotalSpawnText = default;

    [SerializeField]
    Slider Slider = default;

    int spawnCount = 1;
    int totalSpawnCount = 0;

    private EntityManager entityManager => World.DefaultGameObjectInjectionWorld.EntityManager;

    void Awake()
    {
        if (Button == null || Slider == null)
        {
            return;
        }
        Button.onClick.AddListener(Spawn);
        Slider.onValueChanged.AddListener(UpdateSpawnCount);
    }

    void UpdateSpawnCount(float count)
    {
        spawnCount = (int)count;
        if (SpawnText == null)
        {
            return;
        }
        SpawnText.text = "Spawn " + spawnCount;
        if (spawnCount == 1)
        {
            SpawnText.text += " Firefly";
        }
        else
        {
            SpawnText.text += " Fireflies";
        }
    }

    void Spawn()
    {
        var fireflyHabitatArray = entityManager.CreateEntityQuery(typeof(FireFlyHabitat)).ToEntityArray(Allocator.Temp);
        var habitatReference = fireflyHabitatArray[0];

        var random = new Unity.Mathematics.Random((uint)new System.Random().Next());
        var fireflyHabitatSettings = entityManager.GetComponentData<FireFlyHabitat>(habitatReference);
        var outputEntities = new NativeArray<Entity>(spawnCount, Allocator.Temp);
        entityManager.Instantiate(fireflyHabitatSettings.fireflyPrefabEntity, outputEntities);
        for (var i = 0; i < outputEntities.Length; ++i)
        {
            var fireFlyEntity = outputEntities[i];
            var fireflySettings = entityManager.GetComponentData<FireFlySettings>(fireFlyEntity);
            fireflySettings.fireflySpeed = random.NextFloat(fireflyHabitatSettings.fireflyMinimumSpeed, fireflyHabitatSettings.fireflyMaximumSpeed);
            fireflySettings.habitatReference = habitatReference;
            var spawnPosition = random.NextFloat3(fireflyHabitatSettings.habitatCornerA, fireflyHabitatSettings.habitatCornerB);
            entityManager.SetComponentData(fireFlyEntity, new FireFlyLightingCycle() { Value = random.NextFloat(0.0f, fireflySettings.cycleDuration) });
            entityManager.SetComponentData(fireFlyEntity, new Translation() { Value = spawnPosition });
            entityManager.SetComponentData(fireFlyEntity, fireflySettings);
        }
        outputEntities.Dispose();
        fireflyHabitatArray.Dispose();

        totalSpawnCount += spawnCount;
        TotalSpawnText.text = $"Total FireFlies {totalSpawnCount}";
    }
}
