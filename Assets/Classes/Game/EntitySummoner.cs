using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EntitySummoner : MonoBehaviour

{

    public static List<Transform> EnemiesInGameTransform;

    public static List<Enemy> EnemiesInGame;

    public static Dictionary<int, GameObject> EnemyPrefabs;

    public static Dictionary<int, Queue<Enemy>> EnemyObjectPools;

    private static bool IsInitialized;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static void Init()
    {
        if (!IsInitialized)
        {
            EnemyPrefabs = new Dictionary<int, GameObject>();
            EnemyObjectPools = new Dictionary<int, Queue<Enemy>>();
            EnemiesInGame = new List<Enemy>();
            EnemiesInGameTransform = new List<Transform>();


            EnemySummonData[] Enemies = Resources.LoadAll<EnemySummonData>("Enemies");
            //Debug.Log(Enemies[0].name);

            foreach (EnemySummonData enemy in Enemies)
            {
                EnemyPrefabs.Add(enemy.EnemyID, enemy.EnemyPrefab);
                EnemyObjectPools.Add(enemy.EnemyID, new Queue<Enemy>());
            }
            IsInitialized = true;
        }
        else
        {
            Debug.Log("EntitySummoner class has already been initiliazed");
        }
    }

    public static Enemy SummonEnemy(int EnemyID)
    {
        Enemy SummonedEnemy = null;

        //Checks if EnemyPrefabs conatains the enemies ID
        if (EnemyPrefabs.ContainsKey(EnemyID))
        {
            Queue<Enemy> ReferencedQueue = EnemyObjectPools[EnemyID];

            //Checks if there are any enemies left within the queue
            if (ReferencedQueue.Count > 0)
            {
                //Dequeue Enemy and initialize
                SummonedEnemy = ReferencedQueue.Dequeue();
                SummonedEnemy.Init();

                SummonedEnemy.gameObject.SetActive(true);
            }
            else
            {
                //Initialiate new instance of enemy based off of the prefab and initialize
                GameObject NewEnemy = Instantiate(EnemyPrefabs[EnemyID], GameLoopManager.NodePositions[0], Quaternion.identity);
                SummonedEnemy = NewEnemy.GetComponent<Enemy>();
                SummonedEnemy.Init();
            }
        }
        else
        {
            //Debug.Log($"EntitirySummoner: enemy with the id of {EnemyID} does not exist.");
            return null;
        }
        //if(!EnemiesInGame.Contains(SummonedEnemy)) EnemiesInGameTransform.Add(SummonedEnemy.transform);
        //if(!EnemiesInGameTransform.Contains(SummonedEnemy.transform)) EnemiesInGame.Add(SummonedEnemy);
        EnemiesInGameTransform.Add(SummonedEnemy.transform);
        EnemiesInGame.Add(SummonedEnemy);
        SummonedEnemy.ID = EnemyID;
        return SummonedEnemy;
    }

    public static void RemoveEnemy(Enemy EnemyToRemove)
    {
        //ObjectPools should help reduce lag in the late game (Supposedly) because it keeps an instance on the processor and the game itself
        EnemyObjectPools[EnemyToRemove.ID].Enqueue(EnemyToRemove);
        EnemyToRemove.gameObject.SetActive(false);
        EnemiesInGameTransform.Remove(EnemyToRemove.transform);
        EnemiesInGame.Remove(EnemyToRemove);
    }
}