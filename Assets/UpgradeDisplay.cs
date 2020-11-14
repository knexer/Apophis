using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeDisplay : MonoBehaviour
{
    [SerializeField] private Text Name;
    [SerializeField] private Text Description;
    [SerializeField] private Text Cost;
    [SerializeField] private Text TimeToGet;

    [HideInInspector] public Simulation Simulation;
    [HideInInspector] public Upgrade Upgrade;

    private void Start()
    {
        Name.text = Upgrade.Name;
        Description.text = Upgrade.Description;
        Cost.text = Upgrade.Cost.Select(cost => $"{cost.Amount} {cost.Type}")
            .Aggregate((aggregate, cost) => $"{aggregate}, {cost}");
        GetComponent<Button>().onClick.AddListener(BuyUpgrade);

        Simulation.OnSimChanged += UpdateDisplay;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        int? timeToGet = Simulation.GetTimeToPurchase(Upgrade);
        if (timeToGet == null)
        {
            TimeToGet.text = "ERR";
            TimeToGet.color = Color.red;
        }
        else
        {
            TimeToGet.text = timeToGet.ToString() + " turns";
            if (timeToGet < 6) TimeToGet.color = Color.green;
            else if (timeToGet < 30) TimeToGet.color = Color.yellow;
            else TimeToGet.color = Color.Lerp(Color.red, Color.yellow, .5f);
        }

        GetComponent<Button>().interactable = timeToGet != null;
    }

    private void BuyUpgrade()
    {
        Simulation.BuyUpgrade(Upgrade);
    }
}
