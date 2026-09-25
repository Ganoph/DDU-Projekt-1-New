using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.Jobs;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;

public class GameLoopManager : MonoBehaviour
{
    // Variables 
    public static List<TowerBehaviour> TowersInGame;
    public static Vector3[] NodePositions;
    public static float[] NodeDistances;
    private static Queue<EnemyDamageData> DamageData;
    private static Queue<Enemy> EnemiesToRemove;
    private static Queue<int> EnemyIDsToSummon;

    private PlayerStats PlayerStatistics;

    public Transform NodeParent;

    public bool EndLoop;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        PlayerStatistics = FindObjectOfType<PlayerStats>();
        DamageData = new Queue<EnemyDamageData>();
        TowersInGame = new List<TowerBehaviour>();
        EnemyIDsToSummon = new Queue<int>();
        EnemiesToRemove = new Queue<Enemy>();
        EntitySummoner.Init();

        NodePositions = new Vector3[NodeParent.childCount];
        for (int i = 0; i < NodePositions.Length; i++)
        {
            NodePositions[i] = NodeParent.GetChild(i).position;
        }

        NodeDistances = new float[NodePositions.Length - 1];
        for (int i = 0; i < NodeDistances.Length; i++)
        {
            NodeDistances[i] = Vector3.Distance(NodePositions[i], NodePositions[i + 1]);
        }


        StartCoroutine(GameLoop());
        InvokeRepeating("SummonTest", 0f, 3f);
        //InvokeRepeating("RemoveTest", 0f, 0.5f);
    }

    //void RemoveTest()
    //{
    //    if(EntitySummoner.EnemiesInGame.Count > 0)
    //    {
    //        EntitySummoner.RemoveEnemy(EntitySummoner.EnemiesInGame[Random.Range(0, EntitySummoner.EnemiesInGame.Count)]);
    //    }
    //

    void SummonTest()
    {
        EnqueueEnemyIDToSummon(1);
    }

    IEnumerator GameLoop()
    {
        Debug.Log("GameLoop started");
        while (EndLoop == false)
        {
            Debug.Log("GameLoop tick");

            //Spawn Enemies
            if (EnemyIDsToSummon.Count > 0)
            {
                for (int i = 0; i < EnemyIDsToSummon.Count; i++)
                {
                    EntitySummoner.SummonEnemy(EnemyIDsToSummon.Dequeue());
                }
            }

            //Spawn Towers
            
            //Move Enemies
            NativeArray<Vector3> NodesToFollow = new NativeArray<Vector3>(NodePositions, Allocator.TempJob);
            NativeArray<int> NodeIndices = new NativeArray<int>(EntitySummoner.EnemiesInGame.Count, Allocator.TempJob);
            NativeArray<float> EnemySpeed = new NativeArray<float>(EntitySummoner.EnemiesInGame.Count, Allocator.TempJob);
            TransformAccessArray EnemyAccess = new TransformAccessArray(EntitySummoner.EnemiesInGameTransform.ToArray(), 2);

            for (int i = 0; i < EntitySummoner.EnemiesInGame.Count; i++)
            {
                EnemySpeed[i] = EntitySummoner.EnemiesInGame[i].Speed;
                NodeIndices[i] = EntitySummoner.EnemiesInGame[i].NodeIndex;
            }

            MoveEnemiesJob MoveJob = new MoveEnemiesJob
            {
                NodePositions = NodesToFollow,
                EnemySpeed = EnemySpeed,
                NodeIndex = NodeIndices,
                deltaTime = Time.deltaTime
            };

            JobHandle MoveJobHandle = MoveJob.Schedule(EnemyAccess);
            MoveJobHandle.Complete();

            for (int i = 0; i < EntitySummoner.EnemiesInGame.Count; i++)
            {
                EntitySummoner.EnemiesInGame[i].NodeIndex = NodeIndices[i];

                if (EntitySummoner.EnemiesInGame[i].NodeIndex == NodePositions.Length)
                {
                    EnqueueEnemyToRemove(EntitySummoner.EnemiesInGame[i]);
                }
            }

            NodesToFollow.Dispose();
            NodeIndices.Dispose();
            EnemySpeed.Dispose();
            EnemyAccess.Dispose();

            //TickTowers
            //Debug.Log($"Towers in game: {TowersInGame.Count}");
            foreach (TowerBehaviour tower in TowersInGame)
            {

                tower.Target = TowerTargetting.GetTarget(tower, TowerTargetting.TargetType.First);
                tower.Tick();
            }

            //Damage Enemies
            if (DamageData.Count > 0)
            {
                int count = DamageData.Count;
                for (int i = 0; i < count; i++)
                {
                    EnemyDamageData CurrentDamageData = DamageData.Dequeue();
                    CurrentDamageData.TargetedEnemy.Health -= CurrentDamageData.TotalDamage / CurrentDamageData.Resistance;
                    PlayerStatistics.AddMoney((int)CurrentDamageData.TotalDamage);

                    //Removes enemy
                    if (CurrentDamageData.TargetedEnemy.Health <= 0f)
                    {
                        EnqueueEnemyToRemove(CurrentDamageData.TargetedEnemy);
                    }
                }
            }

            //Remove Enemies
            if (EnemiesToRemove.Count > 0)
            {
                int count = EnemiesToRemove.Count;
                for (int i = 0; i < count; i++) 
                {
                    EntitySummoner.RemoveEnemy(EnemiesToRemove.Dequeue());
                }
            }
            //Remove Towers
            yield return null;

        }
    }

    public static void EnqueueDamageData(EnemyDamageData damagedata)
    {
        DamageData.Enqueue(damagedata);
    }

    public static void EnqueueEnemyIDToSummon(int ID)
    {
        EnemyIDsToSummon.Enqueue(ID);
    }

    public static void EnqueueEnemyToRemove(Enemy EnemyToRemove)
    {
        EnemiesToRemove.Enqueue(EnemyToRemove);
    }
}

public struct EnemyDamageData
{

    public EnemyDamageData(Enemy target, float damage, float resistance)
    {
        TargetedEnemy = target;
        TotalDamage = damage;
        Resistance = resistance;
    }

    public Enemy TargetedEnemy;
    public float TotalDamage;
    public float Resistance;
}

public struct MoveEnemiesJob : IJobParallelForTransform
{
    [NativeDisableParallelForRestriction]
    public NativeArray<int> NodeIndex;

    [NativeDisableParallelForRestriction]
    public NativeArray<float> EnemySpeed;

    [NativeDisableParallelForRestriction]
    public NativeArray<Vector3> NodePositions;


    public float deltaTime;


    public void Execute(int index, TransformAccess transform)
    {
        //if (NodeIndex[index] < NodePositions.Length)
        //{

        //}


        Vector3 PositionToMoveTo = NodePositions[NodeIndex[index]];
        transform.position = Vector3.MoveTowards(transform.position, PositionToMoveTo, EnemySpeed[index] * deltaTime);

        if (transform.position == PositionToMoveTo)
        {
            NodeIndex[index]++;
        }
    }
}