using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;


/*
public class FindEnemySystem : ComponentSystem {
    protected override void OnUpdate() {
        Entities.WithNone<HasEnemyComponent>().WithAll<SoldierComponent>().ForEach((Entity soldierEntity, ref Translation soldierTranslation) => {
            // Cycling all solider
            Entity closestEnemyEntity = Entity.Null;
            float3 closestEnemyPos = float3.zero;

            float3 soldierPos = soldierTranslation.Value;

            Entities.WithAll<EnemyComponent>().ForEach((Entity enemyEntity, ref Translation enemyTranslation) => {
                // Cycling all enemy
                float3 enemyPos = enemyTranslation.Value;
                if(closestEnemyEntity == Entity.Null || math.distance(soldierPos, enemyPos) < math.distance(soldierPos, closestEnemyPos)) {
                    closestEnemyEntity = enemyEntity;
                    closestEnemyPos = enemyPos;
                }
            });
            if(closestEnemyEntity != Entity.Null) {
                PostUpdateCommands.AddComponent(soldierEntity, new HasEnemyComponent { enemyEntity = closestEnemyEntity });
            }
        });
    }
}
*/

public class FindEnemyJobSystem : JobComponentSystem {
    private struct EntityWithPosition {
        public Entity entity;
        public float3 position;
    }


    [RequireComponentTag(typeof(SoldierComponent))]
    [ExcludeComponent(typeof(HasEnemyComponent))]
    [BurstCompile]

    private struct FindEnemyJob : IJobForEachWithEntity<Translation> {

        [DeallocateOnJobCompletion][ReadOnly]public NativeArray<EntityWithPosition> enemyEntityWithPositionArray;

        public NativeArray<Entity> closestEntityArray;

        public void Execute(Entity soldierEntity, int index, ref Translation soldierTranslation) {

            // Cycling all solider
            Entity closestEnemyEntity = Entity.Null;
            float3 closestEnemyPos = float3.zero;

            float3 soldierPos = soldierTranslation.Value;

            for(int i = 0; i < enemyEntityWithPositionArray.Length; i ++) {
                Entity enemyEntity = enemyEntityWithPositionArray[i].entity;
                float3 enemyPosition = enemyEntityWithPositionArray[i].position;

                // Cycling all enemy
                float3 enemyPos = enemyPosition;
                if(closestEnemyEntity == Entity.Null || math.distance(soldierPos, enemyPos) < math.distance(soldierPos, closestEnemyPos)) {
                    closestEnemyEntity = enemyEntity;
                    closestEnemyPos = enemyPos;
                }
            }

            closestEntityArray[index] = closestEnemyEntity;

        }
    }


    [RequireComponentTag(typeof(SoldierComponent))]
    [ExcludeComponent(typeof(HasEnemyComponent))]
    private struct AddHasEnemyComponentJob : IJobForEachWithEntity<Translation> {
        public EntityCommandBuffer.Concurrent entityCommandBuffer;
        [DeallocateOnJobCompletion][ReadOnly]public NativeArray<Entity> closestEntityArray;
        public void Execute(Entity soldierEntity, int index, ref Translation c0) {
            Entity closestEnemyEntity = closestEntityArray[index];
            if(closestEnemyEntity != Entity.Null) {
                entityCommandBuffer.AddComponent(index, soldierEntity, new HasEnemyComponent { enemyEntity = closestEnemyEntity });
            }
        }
    }

    EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

    protected override void OnCreate() {
        base.OnCreate();

        endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        EntityQuery enemyEntityQuery = GetEntityQuery(typeof(EnemyComponent), ComponentType.ReadOnly<Translation>());
        NativeArray<Entity> enemyEntityArray = enemyEntityQuery.ToEntityArray(Allocator.TempJob);
        NativeArray<Translation> enemyTranslationArray = enemyEntityQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

        NativeArray<EntityWithPosition> enemyEntityWithPositionArray = new NativeArray<EntityWithPosition>(enemyEntityArray.Length, Allocator.TempJob);

        for(int i = 0; i < enemyEntityArray.Length; i ++) {
            enemyEntityWithPositionArray[i] = new EntityWithPosition {
                entity = enemyEntityArray[i],
                position = enemyTranslationArray[i].Value
            };
        }
        enemyEntityArray.Dispose();
        enemyTranslationArray.Dispose();

        EntityQuery soldierEntityQuery = GetEntityQuery(typeof(SoldierComponent), ComponentType.ReadOnly<Translation>());

        NativeArray<Entity> closestEntityArray = new NativeArray<Entity>(soldierEntityQuery.CalculateEntityCount(), Allocator.TempJob);
        FindEnemyJob findEnemyJob = new FindEnemyJob {
            enemyEntityWithPositionArray = enemyEntityWithPositionArray,
            closestEntityArray = closestEntityArray,
        };


        AddHasEnemyComponentJob addHasEnemyComponentJob = new AddHasEnemyComponentJob {
            closestEntityArray = closestEntityArray,
            entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
        };


        JobHandle jobHandle = findEnemyJob.Schedule(this, inputDeps);

        jobHandle = addHasEnemyComponentJob.Schedule(this, jobHandle);

        endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }
}
