using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    [SerializeField] private ResourceManager resources;
    [SerializeField] private float SimStepAnimationSeconds;

    // Handles the passage of time.
    [HideInInspector] public int CurrentTime = 0;
    public int MaxTime;

    public event Action OnSimChanged;

    public bool IsLocked { get; private set; }

    private IEnumerator FastForwardTo(int nextTime)
    {
        while (CurrentTime < nextTime)
        {
            CurrentTime++;
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                Resource resource = resources.GetResource(type);
                resource.Amount += resource.Growth;
            }
            OnSimChanged?.Invoke();
            yield return new WaitForSeconds(SimStepAnimationSeconds);
        }
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

    private IEnumerator BuyUpgradeCoroutine(Upgrade upgrade)
    {
        IsLocked = true;
        OnSimChanged?.Invoke();
        try
        {
            int? timeToBuy = GetTimeToPurchase(upgrade);
            if (timeToBuy == null)
                throw new ArgumentException($"Upgrade {upgrade.name} can't be afforded before the end of time!");

            yield return FastForwardTo(CurrentTime + timeToBuy.Value);
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

            OnSimChanged?.Invoke();
        }
        finally
        {
            IsLocked = false;
            OnSimChanged?.Invoke();
        }
    }

    public void BuyUpgrade(Upgrade upgrade)
    {
        StartCoroutine(BuyUpgradeCoroutine(upgrade));
    }
}
