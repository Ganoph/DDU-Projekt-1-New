using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField]
    private LayerMask PlacementCheckMask;

    [SerializeField]
    private LayerMask PlacementCollideMask;

    [SerializeField]
    private Camera PlayerCamera;

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
                        GameLoopManager.TowersInGame.Add(
                            CurrentPlacingTower.GetComponent<TowerBehaviour>()
                        );

                        TowerCollider.isTrigger = false;
                        CurrentPlacingTower = null;
                    }
                }
            }
        }
    }

    public void SetTowerToPlace(GameObject tower)
    {
        CurrentPlacingTower = Instantiate(tower, Vector3.zero, Quaternion.identity);
    }
}