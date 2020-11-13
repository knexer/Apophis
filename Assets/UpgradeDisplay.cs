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

    public Upgrade Upgrade;

    private void Start()
    {
        Name.text = Upgrade.Name;
        Description.text = Upgrade.Description;
        Cost.text = Upgrade.Cost.Select(cost => $"{cost.Amount} {cost.Type}")
            .Aggregate((aggregate, cost) => $"{aggregate}, {cost}");
    }
}
