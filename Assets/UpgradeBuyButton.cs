using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeBuyButton : MonoBehaviour
{
    [SerializeField] private GameObject BuyNow;
    [SerializeField] private GameObject SaveUp;
    [SerializeField] private GameObject CannotAfford;
    [SerializeField] private TMP_Text SaveUpCyclesText;

    public void UpdateButton(int? turnsToBuy)
    {
        if (turnsToBuy == null)
        {
            EnableChild(CannotAfford);
        } else if (turnsToBuy > 0)
        {
            EnableChild(SaveUp);
            SaveUpCyclesText.text = $"{turnsToBuy.Value}";
        } else
        {
            EnableChild(BuyNow);
        }
    }

    private void EnableChild(GameObject child)
    {
        GetComponent<Button>().targetGraphic = child.GetComponent<Image>();
        BuyNow.SetActive(child == BuyNow);
        SaveUp.SetActive(child == SaveUp);
        CannotAfford.SetActive(child == CannotAfford);
    }
}
