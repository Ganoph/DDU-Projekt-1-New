using Unity.Collections;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TowerTargetting
{
   
    public enum TargetType
    {
        First,
        Last,
        Close
    }

    public static Enemy GetTarget(TowerBehaviour CurrentTower, TargetType TargetMethod)
    {
        Debug.Log($"Tower range: {CurrentTower.Range}, layer mask value: {CurrentTower.EnemiesLayer.value}");
        Collider[] EnemiesInRange = Physics.OverlapSphere(CurrentTower.transform.position, CurrentTower.Range, CurrentTower.EnemiesLayer);
        Debug.Log($"Enemies in range: {EnemiesInRange.Length}");
        if (EnemiesInRange.Length == 0)
        {
            return null;
        }

        NativeArray<EnemyData> EnemiesToCalculate = new NativeArray<EnemyData>(EnemiesInRange.Length, Allocator.TempJob);
        NativeArray<Vector3> NodePositions = new NativeArray<Vector3>(GameLoopManager.NodePositions, Allocator.TempJob);
        NativeArray<float> NodeDistances = new NativeArray<float>(GameLoopManager.NodeDistances, Allocator.TempJob);
        NativeArray<int> EnemyToIndex = new NativeArray<int>(1, Allocator.TempJob);
        EnemyToIndex[0] = -1;
        int EnemyIndexToReturn = -1;

        for (int i = 0; i < EnemiesToCalculate.Length; i++)
        {
            Enemy CurrentEnemy = EnemiesInRange[i].GetComponentInParent<Enemy>();
            if (CurrentEnemy == null)
            {
                continue;
            }
            int EnemyIndexInList = EntitySummoner.EnemiesInGame.FindIndex(x => x == CurrentEnemy);
            if(EnemyIndexInList == -1)
            {
                continue;
            }
            //Debug.Log($"i:{i}");
            EnemiesToCalculate[i] = new EnemyData(CurrentEnemy.transform.position, CurrentEnemy.NodeIndex, CurrentEnemy.Health,EnemyIndexInList);
            //Debug.Log("***");
        }

        SearchForEnemy EnemySearchJob = new SearchForEnemy
        {
            _EnemiesToCalculate = EnemiesToCalculate,
            _NodeDistances = NodeDistances,
            _NodePositions = NodePositions,
            _EnemyToIndex = EnemyToIndex,
            TargetingType = (int)TargetMethod,
            TowerPosition = CurrentTower.transform.position
        };

        switch ((int)TargetMethod)
        {
            case 0: //first
                EnemySearchJob.CompareValue = Mathf.Infinity;
                break;

            case 1: //Last
                EnemySearchJob.CompareValue = Mathf.NegativeInfinity;
                break;

            case 2: //Close

                goto case 0;
        }

        JobHandle dependency = new JobHandle();
        JobHandle SearchJobHandle = EnemySearchJob.Schedule(EnemiesToCalculate.Length, dependency);

        SearchJobHandle.Complete();


        if (EnemyToIndex[0] != -1)
        {
            EnemyIndexToReturn = EnemiesToCalculate[EnemyToIndex[0]].EnemyIndex;

            EnemiesToCalculate.Dispose();
            NodePositions.Dispose();
            NodeDistances.Dispose();
            EnemyToIndex.Dispose();

            if (EnemyIndexToReturn < 0 || EnemyIndexToReturn >= EntitySummoner.EnemiesInGame.Count)
            {
                return null;
            }
            return EntitySummoner.EnemiesInGame[EnemyIndexToReturn];
        }
        //Debug.Log($"EnemyToIndex[0]: {EnemyToIndex[0]}, EnemyIndexToReturn: {EnemyIndexToReturn}");
        EnemiesToCalculate.Dispose();
        NodePositions.Dispose();
        NodeDistances.Dispose();
        EnemyToIndex.Dispose();
        return null;
    }

    struct EnemyData
    {
        public EnemyData(Vector3 position, int nodeindex, float hp, int enemyIndex)
        {
            EnemyPosition = position;
            NodeIndex = nodeindex;
            EnemyIndex = enemyIndex;
            Health = hp;
        }

        public Vector3 EnemyPosition;
        public int EnemyIndex;
        public int NodeIndex;
        public float Health;
    }

    struct SearchForEnemy : IJobFor
    {
        [NativeDisableParallelForRestriction]
        public NativeArray<EnemyData> _EnemiesToCalculate;

        [NativeDisableParallelForRestriction]
        public NativeArray<Vector3> _NodePositions;

        [NativeDisableParallelForRestriction]
        public NativeArray<float> _NodeDistances;

        [NativeDisableParallelForRestriction]
        public NativeArray<int> _EnemyToIndex;

        public Vector3 TowerPosition;
        public float CompareValue;
        public int TargetingType;

        public void Execute(int index)
        {
            float CurrentEnemyDistanceToEnd = 0;
            float DistanceToEnemy = 0;
            switch (TargetingType)
            {
                case 0: // First


                    CurrentEnemyDistanceToEnd = GetDistanceToEnd(_EnemiesToCalculate[index]);
                    if (CurrentEnemyDistanceToEnd < CompareValue)
                    {
                        _EnemyToIndex[0] = index;
                        CompareValue = CurrentEnemyDistanceToEnd;
                    }

                    break;

                case 1: // Last
                    CurrentEnemyDistanceToEnd = GetDistanceToEnd(_EnemiesToCalculate[index]);
                    if (CurrentEnemyDistanceToEnd > CompareValue)
                    {
                        _EnemyToIndex[0] = index;
                        CompareValue = CurrentEnemyDistanceToEnd;
                    }
                    break;

                case 2: // Close
                    DistanceToEnemy = Vector3.Distance(TowerPosition, _EnemiesToCalculate[index].EnemyPosition);
                    if (DistanceToEnemy < CompareValue)
                    {
                        _EnemyToIndex[0] = index;
                        CompareValue = DistanceToEnemy;
                    }
                    break;
            }
        }

        private float GetDistanceToEnd(EnemyData EnemyToEvaluate)
        {
            int SafeIndex = Mathf.Min(EnemyToEvaluate.NodeIndex, _NodePositions.Length - 1);
            float FinalDistance = Vector3.Distance(EnemyToEvaluate.EnemyPosition, _NodePositions[SafeIndex]);

            for (int i = SafeIndex; i < _NodeDistances.Length; i++)
            {
                Debug.Log($"Final node distance {_NodeDistances[i]}");  
                FinalDistance += _NodeDistances[i];
            }
                
            return FinalDistance;
        }
    }
}