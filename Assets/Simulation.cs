using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    [SerializeField] private ResourceManager resources;

    // Handles the passage of time.
    public int CurrentTime = 0;
    public int MaxTime;

    public void FastForwardTo(int nextTime)
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            Resource resource = resources.GetResource(type);
            resource.Amount += resource.Growth * nextTime - CurrentTime;
        }

        CurrentTime = nextTime;
    }

    public int? GetTimeToPurchase(Upgrade upgrade)
    {
        int? slowestResourceTime = 0;
        foreach (Cost cost in upgrade.Cost)
        {
            int? thisResourceTime;
            Resource resource = resources.GetResource(cost.Type);
            if (cost.Amount < resource.Amount) thisResourceTime = 0;
            else if (resource.Growth <= 0) thisResourceTime = null;
            else thisResourceTime = Mathf.CeilToInt((cost.Amount - resource.Amount) / (float) resource.Growth);

            if (thisResourceTime != null && CurrentTime + thisResourceTime > MaxTime) thisResourceTime = null;

            if (slowestResourceTime == null || thisResourceTime == null) slowestResourceTime = null;
            else if (thisResourceTime > slowestResourceTime) slowestResourceTime = thisResourceTime;
        }

        return slowestResourceTime;
    }
}
