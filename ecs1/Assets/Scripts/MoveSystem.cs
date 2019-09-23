using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class MoveSystem : ComponentSystem {

    protected override void OnUpdate() {
        Entities.ForEach((ref Translation translation, ref MoveSpeedComponent moveSpeedComponent, ref MoveDirComponent moveDirComponent) => {
            if(moveDirComponent.isUp) {
                translation.Value.y += moveSpeedComponent.speed * Time.deltaTime;
            } else {
                translation.Value.y -= moveSpeedComponent.speed * Time.deltaTime;
            }
            if(translation.Value.y > 5) {
                moveDirComponent.isUp = false;
            }

            if(translation.Value.y < -5) {
                moveDirComponent.isUp = true;
            }
        });
    }
}
