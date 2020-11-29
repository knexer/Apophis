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
        GetComponent<ModalWindowManager>().onConfirm.AddListener(StartNewTimeline);
        GetComponent<ModalWindowManager>().onCancel.AddListener(StartNewTimeline);
    }

    private void StartNewTimeline()
    {
        Sim.StartNewTimeline();
        fastForwarding = false;
    }

    private void MaybeShowModal()
    {
        if (fastForwarding == true) return;
        if (Sim.AllUpgrades.All(upgrade => Sim.GetTimeToPurchase(upgrade) == null))
        {
            StartCoroutine(ShowModal());
        }
    }

    private IEnumerator ShowModal()
    {
        fastForwarding = true;
        yield return Sim.FastForwardToEnd();
        GetComponent<ModalWindowManager>().OpenWindow();
    }
}
