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
    private List<Upgrade> boughtUpgrades = new List<Upgrade>();

    [SerializeField, HideInInspector]
    private List<Upgrade> queuedUpgrades = new List<Upgrade>();

    public IEnumerable<Upgrade> UpgradeQueue => boughtUpgrades.Concat(queuedUpgrades);

    public void QueueUpgrades(IEnumerable<Upgrade> upgrades)
    {
        queuedUpgrades.AddRange(upgrades);

        BuyQueuedUpgrades();
        RecalculateNextCycle();
    }

    private void RecalculateNextCycle()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            resources.GetResource(type).ChangeNextCycle = 0;
        }

        foreach (Upgrade upgrade in boughtUpgrades)
        {
            upgrade.Apply(this, resources);
        }
    }

    public void AdvanceTime()
    {
        CurrentTime++;

        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            Resource resource = resources.GetResource(type);
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

        RecalculateNextCycle();
    }
}
