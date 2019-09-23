using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;



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
