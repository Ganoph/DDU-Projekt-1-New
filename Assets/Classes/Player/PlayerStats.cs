using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private UIBehavior uiBehavior;

    [SerializeField]
    private int StartingMoney = 0;

    private int CurrentMoney;

    private void Start()
    {
        CurrentMoney = StartingMoney;
        uiBehavior.UpdateMoney(CurrentMoney);
    }

    public void AddMoney(int MoneyToAdd)
    {
        CurrentMoney += MoneyToAdd;
        uiBehavior.UpdateMoney(CurrentMoney);
    }

    public int GetMoney()
    {
        return CurrentMoney;
    }
}