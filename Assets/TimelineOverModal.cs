using Michsky.UI.ModernUIPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimelineOverModal : MonoBehaviour
{
    [SerializeField] private SimulationManager Sim;

    private bool fastForwarding = false;

    // Start is called before the first frame update
    void Start()
    {
        Sim.OnSimChanged += MaybeShowModal;
    }

    private void MaybeShowModal()
    {
        if (fastForwarding == true) return;
        // If can't buy any upgrades
        if (Sim.AvailableUpgrades.All(upgrade => Sim.GetTimeToPurchase(upgrade) == null))
        {
            StartCoroutine(ShowModal());
        }
        // Fast-forward to end
        // Open modal
    }

    private IEnumerator ShowModal()
    {
        fastForwarding = true;
        yield return Sim.FastForwardToEnd();
        GetComponent<ModalWindowManager>().OpenWindow();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
