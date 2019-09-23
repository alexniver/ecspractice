using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

public class Testing : MonoBehaviour {
    [SerializeField]
    private bool useJob;

    [SerializeField]
    private Transform pfZombie;

    private List<Zombie> zombieList;

    public class Zombie {
        public Transform transform;
        public float moveY;
    }

    private void Start() {
        zombieList = new List<Zombie>();
        for (int i = 0; i < 1000; i ++) {
            Transform zombieTransform = Instantiate(pfZombie, new Vector3(UnityEngine.Random.Range(-8f, 8f), UnityEngine.Random.Range(-5f, 5f)), Quaternion.identity).transform;
            zombieList.Add(new Zombie {
                transform = zombieTransform,
                moveY = UnityEngine.Random.Range(1f, 2f)
            });
        }
    }


    private void Update() {
        float startTime = Time.realtimeSinceStartup;

        if (useJob) {
            TransformAccessArray transformAccesseArray = new TransformAccessArray(zombieList.Count);
            NativeArray<float> moveYList = new NativeArray<float>(zombieList.Count, Allocator.TempJob);
            for (int i = 0;  i < zombieList.Count; i ++) {
                transformAccesseArray.Add(zombieList[i].transform);
                moveYList[i] = zombieList[i].moveY;
            }

            ZombieMoveJobTransform zombieMoveJob = new ZombieMoveJobTransform {
                deltaTime = Time.deltaTime,
                moveYList = moveYList
            };

            JobHandle jobHandle = zombieMoveJob.Schedule(transformAccesseArray);
            jobHandle.Complete();

            for (int i = 0;  i < zombieList.Count; i ++) {
                zombieList[i].moveY = moveYList[i];
            }

            transformAccesseArray.Dispose();
            moveYList.Dispose();
            /*NativeArray<float3> posList = new NativeArray<float3>(zombieList.Count, Allocator.TempJob);
            NativeArray<float> moveYList = new NativeArray<float>(zombieList.Count, Allocator.TempJob);
            for (int i = 0;  i < zombieList.Count; i ++) {
                posList[i] = zombieList[i].transform.position;
                moveYList[i] = zombieList[i].moveY;
            }

            ZombieMoveJob zombieMoveJob = new ZombieMoveJob {
                deltaTime = Time.deltaTime,
                posList = posList,
                moveYList = moveYList
            };

            JobHandle jobHandle = zombieMoveJob.Schedule(zombieList.Count, 100);
            jobHandle.Complete();

            for (int i = 0;  i < zombieList.Count; i ++) {
                zombieList[i].transform.position = posList[i];
                zombieList[i].moveY = moveYList[i];
            }

            posList.Dispose();
            moveYList.Dispose();*/

        } else {
            foreach(Zombie zombie in zombieList) {
                zombie.transform.position += new Vector3(0, zombie.moveY * Time.deltaTime, 0);
                if(zombie.transform.position.y > 5f) {
                    zombie.moveY = -math.abs(zombie.moveY);
                }
                if(zombie.transform.position.y < -5f) {
                    zombie.moveY = math.abs(zombie.moveY);
                }

                float value = 0;
                for(int i = 0; i < 1000; i ++) {
                    value = math.exp10(math.sqrt(value));
                }
            }

        }

       /*NativeList<JobHandle> jobHandles = new NativeList<JobHandle>(Allocator.Temp);
        if(useJob) {
            for (int i = 0; i < 10; i ++) {
                //JobHandle jobHandle = ReallyToughJob();
                // jobHandles.Add(jobHandle);
            }
            JobHandle.CompleteAll(jobHandles);
            jobHandles.Dispose();
        } else {
            for (int i = 0; i < 10; i ++) {
                ReallyToughTask();
            }
        }*/
        Debug.Log("time : " + ((Time.realtimeSinceStartup - startTime) * 1000) + " ms");
    }

    private void ReallyToughTask() {
        float value = 0;
        for(int i = 0; i < 100000; i ++) {
            value = math.exp10(math.sqrt(value));
        }
    }

    private JobHandle ReallyToughJob() {
        ReallyToughJob job = new ReallyToughJob();
        return job.Schedule();
    }

}

[BurstCompile]
public struct ReallyToughJob : IJob {
    public void Execute() {
        float value = 0;
        for(int i = 0; i < 100000; i ++) {
            value = math.exp10(math.sqrt(value));
        }
    }
}

[BurstCompile]
public struct ZombieMoveJob : IJobParallelFor {
    public NativeArray<float3> posList;
    public NativeArray<float> moveYList;
    public float deltaTime;
    public void Execute(int index) {
        posList[index] += new float3(0, moveYList[index] * deltaTime, 0);
        if(posList[index].y > 5f) {
            moveYList[index] = -math.abs(moveYList[index]);
        }
        if(posList[index].y < -5f) {
            moveYList[index] = math.abs(moveYList[index]);
        }

        float value = 0;
        for(int i = 0; i < 1000; i ++) {
            value = math.exp10(math.sqrt(value));
        }

    }
}

[BurstCompile]
public struct ZombieMoveJobTransform : IJobParallelForTransform {
    public NativeArray<float> moveYList;
    public float deltaTime;

    public void Execute(int index, TransformAccess transform) {
        transform.position += new Vector3(0, moveYList[index] * deltaTime, 0);
        if(transform.position.y > 5f) {
            moveYList[index] = -math.abs(moveYList[index]);
        }
        if(transform.position.y < -5f) {
            moveYList[index] = math.abs(moveYList[index]);
        }

        float value = 0;
        for(int i = 0; i < 1000; i ++) {
            value = math.exp10(math.sqrt(value));
        }


    }
}
