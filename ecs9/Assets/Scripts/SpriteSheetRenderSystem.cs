using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

[UpdateAfter(typeof(SpriteSheetAnimSystem))]
public class SpriteSheetRenderSystem : ComponentSystem {
    protected override void OnUpdate() {
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        Vector4[] uv = new Vector4[1];
        Entities.ForEach((ref Translation translation, ref SpriteSheetAnimData spriteSheetAnimData) => {
            uv[0] = spriteSheetAnimData.uv;
            materialPropertyBlock.SetVectorArray("_MainTex_UV", uv);
            Graphics.DrawMesh(GameHandler.GetInstance().quadMesh, 
                spriteSheetAnimData.matrix,
                GameHandler.GetInstance().walkingSpriteSheetMaterial, 
                0, // Layer
                Camera.main,
                0, // Submesh index
                materialPropertyBlock
                );
        });
    }
}
