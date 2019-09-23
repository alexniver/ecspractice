using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;

public class GameHandler : MonoBehaviour {
    [SerializeField]
    private Mesh quadMesh;
    [SerializeField]
    private Material soldierMaterial;
    [SerializeField]
    private Material enemyMaterial;

    private RenderMesh soldierRenderMash;
    private RenderMesh enemyRenderMash;

    private EntityManager entityManager;

    private float spawnTimer;
    private float spawnTimerMax;

    private void Start() {
        spawnTimer = 0.0f;
        spawnTimerMax = 1.0f;


        entityManager = World.Active.EntityManager;
        soldierRenderMash = new RenderMesh { material = soldierMaterial, mesh = quadMesh };
        enemyRenderMash = new RenderMesh { material = enemyMaterial, mesh = quadMesh };


        for(int i = 0; i < 5; i ++) {
            SpawnSoldier();
        }
        for (int i = 0; i < 50; i++) {
            SpawnEnemy();
        }
    }

    private void Update() {
        spawnTimer += Time.deltaTime;
        if(spawnTimer >= spawnTimerMax) {
            spawnTimer = 0.0f;
            SpawnEnemy();
        }
    }

    private void SpawnSoldier() {
        Entity entity = entityManager.CreateEntity(
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(Translation),
            typeof(Scale),
            typeof(SoldierComponent)
            );
        entityManager.SetSharedComponentData(entity, soldierRenderMash);
        entityManager.SetComponentData(entity, new Translation {
            Value = new float3 { x = UnityEngine.Random.Range(-8f, 8f), y = UnityEngine.Random.Range(-5f, 5f), z = 0 }
        });
        entityManager.SetComponentData(entity, new Scale { Value = 1.5f });
    }

    private void SpawnEnemy() {
        Entity entity = entityManager.CreateEntity(
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(Translation),
            typeof(Scale),
            typeof(EnemyComponent)
            );
        entityManager.SetSharedComponentData(entity, enemyRenderMash);
        entityManager.SetComponentData(entity, new Translation {
            Value = new float3 { x = UnityEngine.Random.Range(-8f, 8f), y = UnityEngine.Random.Range(-5f, 5f), z = 0 }
        });
        entityManager.SetComponentData(entity, new Scale { Value = 0.5f });

    }
}

public struct SoldierComponent : IComponentData { }
public struct EnemyComponent : IComponentData { }
public struct HasEnemyComponent : IComponentData {
    public Entity enemyEntity;
}
