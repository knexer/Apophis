using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeList : MonoBehaviour
{
    [SerializeField] private RectTransform UpgradeDisplayParent;
    [SerializeField] private GameObject UpgradeParent;
    [SerializeField] private UpgradeDisplay UpgradeDisplayPrefab;
    [SerializeField] private SimulationManager Simulation;
    [SerializeField] private Timeline Timeline;

    void Start()
    {
        foreach (Upgrade upgrade in UpgradeParent.GetComponentsInChildren<Upgrade>())
        {
            UpgradeDisplay upgradeDisplay = Instantiate(UpgradeDisplayPrefab, UpgradeDisplayParent, false);
            upgradeDisplay.Upgrade = upgrade;
            upgradeDisplay.Simulation = Simulation;
            upgradeDisplay.Timeline = Timeline;
        }
    }
}
