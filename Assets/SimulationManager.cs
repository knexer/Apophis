using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [SerializeField] public SimAggregate ActualSims;
    [SerializeField] private float SimStepAnimationSeconds;
    [SerializeField] private GameObject UpgradeContainer;
    public bool IsLocked { get; private set; }
    public event Action OnSimChanged;
    public IEnumerable<Upgrade> AvailableUpgrades => UpgradeContainer.GetComponentsInChildren<Upgrade>();

    private readonly Dictionary<Upgrade, int?> memoizedUpgradeTimes = new Dictionary<Upgrade, int?>();

    private void Start()
    {
        StartNewTimeline();
    }

    private void UpdateUpgradePurchaseTimes()
    {
        // TODO do one sim forward, collecting upgrade times as you go, instead of one per upgrade
        memoizedUpgradeTimes.Clear();
        foreach (var upgrade in AvailableUpgrades)
        {
            memoizedUpgradeTimes.Add(upgrade, GetTimeToPurchaseImpl(upgrade));
        }
    }

    private void SimChanged()
    {
        UpdateUpgradePurchaseTimes();
        OnSimChanged?.Invoke();
    }

    private IEnumerator FastForwardTo(int nextTime)
    {
        while (ActualSims.CurrentSim.CurrentTime < nextTime)
        {
            ActualSims.AdvanceTime();
            SimChanged();
            yield return new WaitForSeconds(SimStepAnimationSeconds);
        }
    }
    public void StartNewTimeline()
    {
        ActualSims.StartNewTimeline();
        SimChanged();
    }

    public IEnumerator FastForwardToEnd()
    {
        yield return FastForwardTo(ActualSims.CurrentSim.MaxTime);
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

            yield return FastForwardTo(ActualSims.CurrentSim.CurrentTime + timeToBuy.Value);

            ActualSims.CurrentSim.BuyUpgrade(upgrade);
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
        if (ActualSims.CurrentSim.CanBuyUpgrade(upgrade)) return 0;
        var copy = Instantiate(ActualSims.gameObject).GetComponent<SimAggregate>();
        try
        {
            while (!copy.CurrentSim.CanBuyUpgrade(upgrade) && copy.CurrentSim.CurrentTime < copy.CurrentSim.MaxTime)
            {
                copy.AdvanceTime();
            }

            if (copy.CurrentSim.CanBuyUpgrade(upgrade)) return copy.CurrentSim.CurrentTime - ActualSims.CurrentSim.CurrentTime;
            return null;
        }
        finally
        {
            Destroy(copy.gameObject);
        }
    }

    public int? GetTimeToPurchase(Upgrade upgrade)
    {
        if (memoizedUpgradeTimes.ContainsKey(upgrade))
            return memoizedUpgradeTimes[upgrade];
        return null;
    }
}
