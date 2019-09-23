using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

public class SpriteSheetAnimSystem : JobComponentSystem {
    [BurstCompile]
    public struct SpriteSheetAnimJob : IJobForEach<SpriteSheetAnimData, Translation> {
        public float deltaTime;
        public void Execute(ref SpriteSheetAnimData spriteSheetAnimData, ref Translation translation) {
            spriteSheetAnimData.frameTimer += deltaTime;
            while(spriteSheetAnimData.frameTimer >= spriteSheetAnimData.frameTimerMax) {
                spriteSheetAnimData.frameTimer -= spriteSheetAnimData.frameTimerMax;
                spriteSheetAnimData.currentFrame = (spriteSheetAnimData.currentFrame + 1) % spriteSheetAnimData.totalFrame;

                float uvWidth = 1.0f / spriteSheetAnimData.totalFrame;
                float uvHeight = 1f;
                float uvOffsetX = spriteSheetAnimData.currentFrame*uvWidth;
                float uvOffsetY = 0f;
                spriteSheetAnimData.uv = new Vector4(uvWidth, uvHeight, uvOffsetX, uvOffsetY);

                float3 position = translation.Value;
                position.z = position.y * 0.01f;
                spriteSheetAnimData.matrix = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);
            }
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        SpriteSheetAnimJob spriteSheetAnimJob = new SpriteSheetAnimJob {
            deltaTime = Time.deltaTime
        };

        return spriteSheetAnimJob.Schedule(this, inputDeps);

    }
}

public struct SpriteSheetAnimData : IComponentData {
    public int currentFrame;
    public int totalFrame;
    public float frameTimer;
    public float frameTimerMax;
    public Vector4 uv;
    public Matrix4x4 matrix;
}
