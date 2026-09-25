using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI MoneyDisplayText;

    [SerializeField]
    private int StartingMoney;

    private int CurrentMoney;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        CurrentMoney = StartingMoney;
        MoneyDisplayText.SetText($"${StartingMoney}");
    }

    public void AddMoney(int MoneyToAdd)
    {
        CurrentMoney += MoneyToAdd;
        MoneyDisplayText.SetText($"${CurrentMoney}");
    }

    public int GetMoney()
    {
        return CurrentMoney;
    }

}