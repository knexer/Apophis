using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpecializationDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text Name;
    [SerializeField] private UpgradeDisplay UpgradeDisplayPrefab;
    [SerializeField] private RectTransform UpgradeDisplayParent;

    public void Init(Specialization spec, SimulationManager simulation, ParallelTimelineDisplay timelines)
    {
        Name.text = spec.Name;
        foreach (Upgrade upgrade in spec.Upgrades)
        {
            UpgradeDisplay upgradeDisplay = Instantiate(UpgradeDisplayPrefab, UpgradeDisplayParent, false);
            upgradeDisplay.Upgrade = upgrade;
            upgradeDisplay.Simulation = simulation;
            upgradeDisplay.Timelines = timelines;
        }
    }
}
