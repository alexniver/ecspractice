using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Collections;

public class Testing : MonoBehaviour {

    [SerializeField]
    private Material material;


    private EntityManager entityManager;

    // Start is called before the first frame update
    private void Start() {
        entityManager = World.Active.EntityManager;

        

        RenderMesh renderMesh = new RenderMesh { mesh = CreateMesh(0.4f, 1), material = material };
        //NonUniformScale nonUniformScale = new NonUniformScale { Value = new float3 { x = 1, y = 2, z = 1 } };

        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(Translation),
            typeof(Rotation)
            //typeof(NonUniformScale)
            );

        NativeArray<Entity> entityArray = new NativeArray<Entity>(10, Allocator.Temp);
        entityManager.CreateEntity(entityArchetype, entityArray);
        foreach(Entity entity in entityArray) {
            entityManager.SetComponentData(entity, new Translation { Value = new float3 { x=UnityEngine.Random.Range(-8, 8), y=UnityEngine.Random.Range(-5, 5), z=0 } });
            entityManager.SetSharedComponentData(entity, renderMesh);
            //entityManager.SetComponentData(entity, nonUniformScale);
        }
        entityArray.Dispose();
    }

    private Mesh CreateMesh(float width, float height) {
        Vector3[] vertices = new Vector3[4];
        Vector2[] uv = new Vector2[4];
        int[] triangles = new int[6];

        /*
         * 0,0
         * 0,1
         * 1,1
         * 1,0
         */

        float halfWidth = width / 2;
        float halfHeight = height / 2;

        vertices[0] = new Vector3(-halfWidth, -halfHeight);
        vertices[1] = new Vector3(-halfWidth, +halfHeight);
        vertices[2] = new Vector3(+halfWidth, +halfHeight);
        vertices[3] = new Vector3(+halfWidth, -halfHeight);

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(0, 1);
        uv[2] = new Vector2(1, 1);
        uv[3] = new Vector2(1, 0);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 3;
        triangles[3] = 1;
        triangles[4] = 2;
        triangles[5] = 3;

        Mesh result = new Mesh();
        result.vertices = vertices;
        result.uv = uv;
        result.triangles = triangles;

        return result;
    }
}

public class MoveSystem : ComponentSystem {
    protected override void OnUpdate() {
        Entities.ForEach((ref Translation translation) => {
            translation.Value.y += 1f * Time.deltaTime;
        });
    }
}

public class RotateSystem : ComponentSystem {
    protected override void OnUpdate() {
        Entities.ForEach((ref Rotation rotation) => {
            rotation.Value = quaternion.Euler(0, 0, math.PI * Time.realtimeSinceStartup);
        });
    }
}

/* 可以使用system设置scale, 也可以在start里设置
public class ScaleSystem : ComponentSystem {
    protected override void OnUpdate() {
        Entities.ForEach((ref NonUniformScale nonUniformScale) => {
            nonUniformScale.Value = new float3(1, 2, 1);
        });
    }
}
*/

