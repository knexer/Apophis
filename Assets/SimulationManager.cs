using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [SerializeField] public Simulation Sim;
    [SerializeField] private float SimStepAnimationSeconds;
    public bool IsLocked { get; private set; }
    public event Action OnSimChanged;

    private IEnumerator FastForwardTo(int nextTime)
    {
        while (Sim.CurrentTime < nextTime)
        {
            Sim.AdvanceTime();
            OnSimChanged?.Invoke();
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
            OnSimChanged?.Invoke();
        }
        finally
        {
            IsLocked = false;
            OnSimChanged?.Invoke();
        }
    }

    public int? GetTimeToPurchase(Upgrade upgrade)
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
}
