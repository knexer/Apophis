using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    [SerializeField] public ResourceManager resources;

    // Handles the passage of time.
    [HideInInspector] public int CurrentTime = 0;
    public int MaxTime;

    [SerializeField, HideInInspector]
    public List<Upgrade> boughtUpgrades = new List<Upgrade>();
    public List<int> boughtUpgradeTimes = new List<int>();

    [SerializeField, HideInInspector]
    private List<Upgrade> queuedUpgrades = new List<Upgrade>();

    public IEnumerable<Upgrade> UpgradeQueue => boughtUpgrades.Concat(queuedUpgrades);

    public Func<IEnumerable<Upgrade>> AllUpgradesCallback;

    public void QueueUpgrades(IEnumerable<Upgrade> upgrades)
    {
        queuedUpgrades.AddRange(upgrades);

        BuyQueuedUpgrades();
        RecalculateNextCycle();
    }

    public void RecalculateNextCycle()
    {
        foreach (Resource resource in resources.Resources)
        {
            resource.ChangeNextCycle = 0;
        }

        foreach (Upgrade upgrade in AllUpgradesCallback())
        {
            upgrade.Apply(this, resources);
        }
    }

    public void AdvanceTime()
    {
        CurrentTime++;

        foreach (Resource resource in resources.Resources)
        {
            resource.Amount += resource.ChangeNextCycle;
        }

        BuyQueuedUpgrades();
        RecalculateNextCycle();
    }

    private void BuyQueuedUpgrades()
    {
        while (queuedUpgrades.Count > 0 && CanBuyUpgrade(queuedUpgrades.First()))
        {
            BuyUpgrade(queuedUpgrades.First());
            queuedUpgrades.RemoveAt(0);
        }
    }

    public bool CanBuyUpgrade(Upgrade upgrade)
    {
        return upgrade.Cost.All(resources.CanApplyImmediate);
    }

    public void BuyUpgrade(Upgrade upgrade)
    {
        foreach (ResourceDelta cost in upgrade.Cost)
        {
            if (!resources.CanApplyImmediate(cost))
                throw new Exception(
                    $"Expected at least {cost.Amount} of {cost.Type}, found only {resources.GetResource(cost.Type).Amount}." +
                    $"This should not happen, because we should have fastforwarded until there was enough!");
            resources.ApplyImmediate(cost);
        }

        boughtUpgrades.Add(upgrade);
        boughtUpgradeTimes.Add(CurrentTime);

        RecalculateNextCycle();
    }
}
