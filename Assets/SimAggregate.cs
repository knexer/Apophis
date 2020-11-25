using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimAggregate : MonoBehaviour
{
    [SerializeField] private Simulation SimPrefab;
    [SerializeField] private Upgrade InitialUpgrade;

    [HideInInspector] public Simulation CurrentSim;
    [HideInInspector] public List<Simulation> PreviousSims;

    public void AdvanceTime()
    {
        CurrentSim.AdvanceTime();
        foreach (Simulation previousSim in PreviousSims)
        {
            previousSim.AdvanceTime();
        }
    }

    private IEnumerable<Upgrade> GetCrossTimelineBoughtUpgrades()
    {
        return PreviousSims.Append(CurrentSim).SelectMany(sim => sim.boughtUpgrades);
    }

    public void StartNewTimeline()
    {
        if (CurrentSim != null)
        {
            PreviousSims.Add(CurrentSim);
        }
        CurrentSim = Instantiate(SimPrefab, transform, false);
        CurrentSim.AllUpgradesCallback = GetCrossTimelineBoughtUpgrades;
        CurrentSim.QueueUpgrades(Enumerable.Repeat(InitialUpgrade, 1));
        CurrentSim.resources.GetResource(ResourceType.Capacity).Amount = 50;
        for (int i = 0; i < PreviousSims.Count; i++)
        {
            Simulation replacementSim = Instantiate(SimPrefab, transform, false);
            replacementSim.AllUpgradesCallback = GetCrossTimelineBoughtUpgrades;
            Simulation previousSim = PreviousSims[i];
            replacementSim.QueueUpgrades(previousSim.UpgradeQueue);
            replacementSim.resources.GetResource(ResourceType.Capacity).Amount = 50;
            PreviousSims[i] = replacementSim;
            Destroy(previousSim.gameObject);
        }

        // Recalculate all next turns
        CurrentSim.RecalculateNextCycle();
        foreach (Simulation previousSim in PreviousSims)
        {
            previousSim.RecalculateNextCycle();
        }
    }

    internal void DoThatHackyThang()
    {
        CurrentSim.AllUpgradesCallback = GetCrossTimelineBoughtUpgrades;
        foreach (Simulation previousSim in PreviousSims)
        {
            previousSim.AllUpgradesCallback = GetCrossTimelineBoughtUpgrades;
        }
    }
}
