using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;

public class MoveToSystem : ComponentSystem {
    protected override void OnUpdate() {
        Entities.WithAll<HasEnemyComponent>().ForEach((Entity soldierEntity, ref Translation soldierTranslation, ref HasEnemyComponent hasEnemyComponent) => {
            if(!World.Active.EntityManager.Exists(hasEnemyComponent.enemyEntity)) {
                PostUpdateCommands.RemoveComponent<HasEnemyComponent>(soldierEntity);
                return;
            }

            Translation enemyTranslation = World.Active.EntityManager.GetComponentData<Translation>(hasEnemyComponent.enemyEntity);

            soldierTranslation.Value += math.normalize(enemyTranslation.Value - soldierTranslation.Value) * 5.0f * Time.deltaTime;
            if(math.distance(soldierTranslation.Value, enemyTranslation.Value) < 0.3f) {
                PostUpdateCommands.DestroyEntity(hasEnemyComponent.enemyEntity);
                PostUpdateCommands.RemoveComponent<HasEnemyComponent>(soldierEntity);
            }
        });
    }
}
