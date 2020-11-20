using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDisplay : MonoBehaviour
{
    [SerializeField] private ResourceType Type;

    [SerializeField] private Text Name;
    [SerializeField] private Text Amount;
    [SerializeField] private Text Growth;

    public SimulationManager Simulation;

    private void Start()
    {
        Name.text = Type.ToString();

        Simulation.OnSimChanged += UpdateResource;
        UpdateResource();
    }

    private void UpdateResource()
    {
        Resource resource = Simulation.ActualSims.CurrentSim.resources.GetResource(Type);
        Amount.text = resource.Amount.ToString();
        Growth.text = $"{(resource.ChangeNextCycle > 0 ? "+" : "")}{resource.ChangeNextCycle}/cycle";
    }
}
