using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [SerializeField] public Simulation Sim;
    [SerializeField] private float SimStepAnimationSeconds;
    [SerializeField] private Upgrade InitialUpgrade;
    public bool IsLocked { get; private set; }
    public event Action OnSimChanged;

    private readonly Dictionary<Upgrade, int?> memoizedUpgradeTimes = new Dictionary<Upgrade, int?>();

    private void Start()
    {
        Sim.QueueUpgrades(Enumerable.Repeat(InitialUpgrade, 1));
        SimChanged();
    }

    private void SimChanged()
    {
        memoizedUpgradeTimes.Clear();
        OnSimChanged?.Invoke();
    }

    private IEnumerator FastForwardTo(int nextTime)
    {
        while (Sim.CurrentTime < nextTime)
        {
            Sim.AdvanceTime();
            SimChanged();
            yield return new WaitForSeconds(SimStepAnimationSeconds);
        }
    }

    public void BuyUpgrade(Upgrade upgrade)
    {
        StartCoroutine(BuyUpgradeCoroutine(upgrade));
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

            yield return FastForwardTo(Sim.CurrentTime + timeToBuy.Value);

            Sim.BuyUpgrade(upgrade);
            SimChanged();
        }
        finally
        {
            IsLocked = false;
            OnSimChanged?.Invoke();
        }
    }

    private int? GetTimeToPurchaseImpl(Upgrade upgrade)
    {
        if (Sim.CanBuyUpgrade(upgrade)) return 0;
        var copy = Instantiate(Sim.gameObject).GetComponent<Simulation>();
        try
        {
            while (!copy.CanBuyUpgrade(upgrade) && copy.CurrentTime < copy.MaxTime)
            {
                copy.AdvanceTime();
            }

            if (copy.CanBuyUpgrade(upgrade)) return copy.CurrentTime - Sim.CurrentTime;
            return null;
        }
        finally
        {
            Destroy(copy.gameObject);
        }
    }

    public int? GetTimeToPurchase(Upgrade upgrade)
    {
        if (!memoizedUpgradeTimes.ContainsKey(upgrade))
            return memoizedUpgradeTimes[upgrade] = GetTimeToPurchaseImpl(upgrade);
        return memoizedUpgradeTimes[upgrade];
    }
}
