using TMPro;
using UnityEngine;

public class GUIShowBalance : MonoBehaviour
{
    private TextMeshProUGUI textGUI;

    private void OnEnable()
    {
        textGUI = GetComponent<TextMeshProUGUI>();
        StoreFinanceEvents.OnBalanceUpdated += UpdateBalance;
    }

    private void OnDisable()
    {
        StoreFinanceEvents.OnBalanceUpdated -= UpdateBalance;
    }

    private void UpdateBalance(float balance)
    {
        if (textGUI == null)
        {
            return;
        }
        textGUI.text = "Balance: $" + balance.ToString();
    }
}