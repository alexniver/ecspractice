using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

public class GameHandler : MonoBehaviour {

    private static GameHandler instance;

    public static GameHandler GetInstance() {
        return instance;
    }

    public Mesh quadMesh;
    public Material walkingSpriteSheetMaterial;
    private void Awake() {
        instance = this;

        EntityManager entityManager = World.Active.EntityManager;
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(SpriteSheetAnimData)
            );

        NativeArray<Entity> entityArray = new NativeArray<Entity>(100, Allocator.Temp);
        entityManager.CreateEntity(entityArchetype, entityArray);

        foreach(Entity entity in entityArray) {
            entityManager.SetComponentData(entity, new Translation {
                Value = new float3(UnityEngine.Random.Range(-8f, 8f), UnityEngine.Random.Range(-5f, 5f), 0)
            });

            entityManager.SetComponentData(entity, new SpriteSheetAnimData {
                currentFrame = UnityEngine.Random.Range(0,4),
                totalFrame = 4,
                frameTimer = 0.0f,
                frameTimerMax = 0.1f
            });
        }

        
        entityArray.Dispose();

    }
}
