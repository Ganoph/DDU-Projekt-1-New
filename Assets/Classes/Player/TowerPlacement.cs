using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField]
    private LayerMask PlacementCheckMask;

    [SerializeField]
    private LayerMask PlacementCollideMask;

    [SerializeField]
    private Camera PlayerCamera;

    [SerializeField]
    private PlayerStats PlayerStatistics;

    // Drag your tower prefab into this field in the Inspector
    [SerializeField]
    private GameObject TowerToPlace;

    private GameObject CurrentPlacingTower;

    void Update()
    {
        // Press 1 to select the tower
        if (Input.GetKeyDown(KeyCode.Alpha1) && CurrentPlacingTower == null)
        {
            SetTowerToPlace(TowerToPlace);
        }

        if (CurrentPlacingTower != null)
        {
            Ray camray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit HitInfo;

            bool hitSomething = Physics.Raycast(camray, out HitInfo, 100f, PlacementCollideMask);

            // Move tower to mouse position
            if (hitSomething)
            {
                CurrentPlacingTower.transform.position = HitInfo.point;
            }

            // Q = cancel placement
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Destroy(CurrentPlacingTower);
                CurrentPlacingTower = null;
                return;
            }

            // E = place tower
            if (Input.GetKeyDown(KeyCode.E) && hitSomething)
            {
                if (!HitInfo.collider.CompareTag("CantPlace"))
                {
                    BoxCollider TowerCollider =
                        CurrentPlacingTower.GetComponent<BoxCollider>();

                    TowerCollider.isTrigger = true;

                    Vector3 BoxCenter =
                        CurrentPlacingTower.transform.position + TowerCollider.center;

                    Vector3 HalfExtents =
                        TowerCollider.size / 2;

                    if (!Physics.CheckBox(BoxCenter, HalfExtents, Quaternion.identity, PlacementCheckMask, QueryTriggerInteraction.Ignore))
                    {
                        TowerBehaviour CurrentTowerBehaviour = CurrentPlacingTower.GetComponent<TowerBehaviour>();
                        GameLoopManager.TowersInGame.Add(CurrentTowerBehaviour);

                        PlayerStatistics.AddMoney(-CurrentTowerBehaviour.SummonCost);

                        TowerCollider.isTrigger = false;
                        CurrentPlacingTower = null;
                    }
                }
            }
        }
    }

    public void SetTowerToPlace(GameObject tower)
    {
        int TowerSummonCost = tower.GetComponent<TowerBehaviour>().SummonCost;

        if(PlayerStatistics.GetMoney() >= TowerSummonCost)
        {
            CurrentPlacingTower = Instantiate(tower, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.Log("You need more money to place this tower down");
        }
    }
}