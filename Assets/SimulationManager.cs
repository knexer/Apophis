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
        memoizedUpgradeTimes.Clear();
        foreach (Upgrade upgrade in AvailableUpgrades)
        {
            if (memoizedUpgradeTimes.ContainsKey(upgrade)) continue;
            if (ActualSims.CurrentSim.CanBuyUpgrade(upgrade)) memoizedUpgradeTimes[upgrade] = ActualSims.CurrentSim.CurrentTime;
        }
        var copy = Instantiate(ActualSims.gameObject).GetComponent<SimAggregate>();
        copy.DoThatHackyThang();
        try
        {
            while (!AvailableUpgrades.All(memoizedUpgradeTimes.ContainsKey) && copy.CurrentSim.CurrentTime < copy.CurrentSim.MaxTime)
            {
                copy.AdvanceTime();
                foreach (Upgrade upgrade in AvailableUpgrades)
                {
                    if (memoizedUpgradeTimes.ContainsKey(upgrade)) continue;
                    if (copy.CurrentSim.CanBuyUpgrade(upgrade)) memoizedUpgradeTimes[upgrade] = copy.CurrentSim.CurrentTime;
                }
            }
        }
        finally
        {
            Destroy(copy.gameObject);
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
            OnSimChanged?.Invoke();
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

    public int? GetTimeToPurchase(Upgrade upgrade)
    {
        if (memoizedUpgradeTimes.ContainsKey(upgrade))
            return memoizedUpgradeTimes[upgrade] - ActualSims.CurrentSim.CurrentTime;
        return null;
    }
}
