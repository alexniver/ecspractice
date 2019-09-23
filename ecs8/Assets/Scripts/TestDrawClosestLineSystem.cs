using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;

public class TestDrawClosestLineSystem : ComponentSystem {
    protected override void OnUpdate() {
        Entities.WithAll<HasEnemyComponent>().ForEach((Entity entity, ref Translation translation, ref HasEnemyComponent hasEnemyComponent) => {
            if(World.Active.EntityManager.Exists(hasEnemyComponent.enemyEntity)) {
                Translation enemyTranslation = World.Active.EntityManager.GetComponentData<Translation>(hasEnemyComponent.enemyEntity);
                Debug.DrawLine(translation.Value, enemyTranslation.Value);
            }
        });
    }
}
