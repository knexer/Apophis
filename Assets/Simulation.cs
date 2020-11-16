using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    [SerializeField] private ResourceManager resources;

    // Handles the passage of time.
    [HideInInspector] public int CurrentTime = 0;
    public int MaxTime;

    public void AdvanceTime()
    {
        CurrentTime++;
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            Resource resource = resources.GetResource(type);
            resource.Amount += resource.Growth;
        }
    }

    public bool CanBuyUpgrade(Upgrade upgrade)
    {
        return upgrade.Cost.All(cost => resources.GetResource(cost.Type).Amount >= cost.Amount);
    }

    public void BuyUpgrade(Upgrade upgrade)
    {
        foreach (Cost cost in upgrade.Cost)
        {
            Resource resource = resources.GetResource(cost.Type);
            if (resource.Amount < cost.Amount)
                throw new Exception(
                    $"Expected at least {cost.Amount} of {cost.Type}, found only {resource.Amount}." +
                    $"This should not happen, because we should have fastforwarded until there was enough!");
            resource.Amount -= cost.Amount;
        }

        upgrade.OnBought(this, resources);
    }
}
