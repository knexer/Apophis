using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [SerializeField] private Simulation SimPrefab;
    [SerializeField] private float SimStepAnimationSeconds;
    [SerializeField] private Upgrade InitialUpgrade;
    [SerializeField] private GameObject UpgradeContainer;
    private List<Simulation> PreviousSims = new List<Simulation>();
    [HideInInspector] public Simulation CurrentSim;
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
        while (CurrentSim.CurrentTime < nextTime)
        {
            CurrentSim.AdvanceTime();
            SimChanged();
            yield return new WaitForSeconds(SimStepAnimationSeconds);
        }
    }
    public void StartNewTimeline()
    {
        if (CurrentSim != null)
        {
            PreviousSims.Add(CurrentSim);
        }
        CurrentSim = Instantiate(SimPrefab);
        CurrentSim.QueueUpgrades(Enumerable.Repeat(InitialUpgrade, 1));
        for (int i = 0; i < PreviousSims.Count; i++){
            Simulation replacementSim = Instantiate(SimPrefab);
            Simulation previousSim = PreviousSims[i];
            replacementSim.QueueUpgrades(previousSim.UpgradeQueue);
            PreviousSims[i] = replacementSim;
            Destroy(previousSim);
        }
        SimChanged();
    }

    public IEnumerator FastForwardToEnd()
    {
        yield return FastForwardTo(CurrentSim.MaxTime);
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

            yield return FastForwardTo(CurrentSim.CurrentTime + timeToBuy.Value);

            CurrentSim.BuyUpgrade(upgrade);
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
        if (CurrentSim.CanBuyUpgrade(upgrade)) return 0;
        var copy = Instantiate(CurrentSim.gameObject).GetComponent<Simulation>();
        try
        {
            while (!copy.CanBuyUpgrade(upgrade) && copy.CurrentTime < copy.MaxTime)
            {
                copy.AdvanceTime();
            }

            if (copy.CanBuyUpgrade(upgrade)) return copy.CurrentTime - CurrentSim.CurrentTime;
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
