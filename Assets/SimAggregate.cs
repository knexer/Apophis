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
        // TODO make these interact
        CurrentSim.AdvanceTime();
        foreach (Simulation previousSim in PreviousSims)
        {
            previousSim.AdvanceTime();
        }
    }

    public void StartNewTimeline()
    {
        if (CurrentSim != null)
        {
            PreviousSims.Add(CurrentSim);
        }
        CurrentSim = Instantiate(SimPrefab, transform, false);
        CurrentSim.QueueUpgrades(Enumerable.Repeat(InitialUpgrade, 1));
        for (int i = 0; i < PreviousSims.Count; i++)
        {
            Simulation replacementSim = Instantiate(SimPrefab, transform, false);
            Simulation previousSim = PreviousSims[i];
            replacementSim.QueueUpgrades(previousSim.UpgradeQueue);
            PreviousSims[i] = replacementSim;
            Destroy(previousSim.gameObject);
        }
    }
}
