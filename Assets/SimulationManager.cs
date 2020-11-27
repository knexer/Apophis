using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [SerializeField] public SimAggregate ActualSims;
    [SerializeField] private float SimStepAnimationSeconds;
    [SerializeField] private float SimStepAnimationSpeedupRatioPerStep;
    [SerializeField] private float SimStepAnimationMinSeconds;
    [SerializeField] private GameObject UpgradeContainer;
    public SimulationHistory CurrentTimelineHistory;
    public List<SimulationHistory> OtherTimelineHistories = new List<SimulationHistory>();

    public bool IsLocked { get; private set; }
    public event Action OnSimChanged;
    public IEnumerable<Upgrade> AvailableUpgrades => UpgradeContainer.GetComponentsInChildren<Upgrade>();

    private readonly Dictionary<Upgrade, int?> memoizedUpgradeTimes = new Dictionary<Upgrade, int?>();

    private void Start()
    {
        StartNewTimeline();
    }

    private void UpdateForecast()
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
            while (copy.CurrentSim.CurrentTime < copy.CurrentSim.MaxTime)
            {
                copy.AdvanceTime();
                int currentTime = copy.CurrentSim.CurrentTime;
                foreach (Upgrade upgrade in AvailableUpgrades)
                {
                    if (memoizedUpgradeTimes.ContainsKey(upgrade)) continue;
                    if (copy.CurrentSim.CanBuyUpgrade(upgrade)) memoizedUpgradeTimes[upgrade] = currentTime;
                }
                CurrentTimelineHistory.SetResourcesAtTime(currentTime, copy.CurrentSim.resources);
                for (int historyIndex = 0; historyIndex < copy.PreviousSims.Count; historyIndex++)
                {
                    OtherTimelineHistories[historyIndex].SetResourcesAtTime(currentTime, copy.PreviousSims[historyIndex].resources);
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
        UpdateForecast();
        OnSimChanged?.Invoke();
    }

    private IEnumerator FastForwardTo(int nextTime)
    {
        float initialTime = Time.time;
        int initialTimeStep = ActualSims.CurrentSim.CurrentTime;
        while (ActualSims.CurrentSim.CurrentTime < nextTime)
        {
            ActualSims.AdvanceTime();
            OnSimChanged?.Invoke();

            float acceleratingSpeed = Mathf.Pow(SimStepAnimationSpeedupRatioPerStep, ActualSims.CurrentSim.CurrentTime - initialTimeStep);
            float deceleratingSpeed = Mathf.Pow(SimStepAnimationSpeedupRatioPerStep, nextTime - ActualSims.CurrentSim.CurrentTime);
            float speed = Mathf.Min(acceleratingSpeed, deceleratingSpeed);
            yield return new WaitForSeconds(Mathf.Max(SimStepAnimationSeconds / speed, SimStepAnimationMinSeconds));
        }
    }
    public void StartNewTimeline()
    {
        ActualSims.StartNewTimeline();
        OtherTimelineHistories.Clear();
        CurrentTimelineHistory = new SimulationHistory(ActualSims.CurrentSim);
        foreach (Simulation simulation in ActualSims.PreviousSims)
        {
            OtherTimelineHistories.Add(new SimulationHistory(simulation));
        }
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
