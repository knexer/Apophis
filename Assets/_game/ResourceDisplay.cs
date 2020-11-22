using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDisplay : MonoBehaviour
{
    [SerializeField] private ResourceType Type;

    [SerializeField] private Image Icon;
    [SerializeField] private TMP_Text Name;
    [SerializeField] private Text Amount;
    [SerializeField] private Text Growth;

    public SimulationManager Simulation;

    public void Init(ResourceType type, SimulationManager simulation)
    {
        Type = type;
        Simulation = simulation;
        ResourceTypeConfig config = FindObjectsOfType<ResourceTypeConfig>().First(maybeConfig => maybeConfig.Type == type);
        Name.text = config.NameAndIcon;
        Icon.sprite = config.BigImage;

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
