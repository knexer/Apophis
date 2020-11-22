using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceUI : MonoBehaviour
{
    [SerializeField] private ResourceDisplay ResourceDisplayPrefab;
    [SerializeField] private SimulationManager Simulation;

    // Start is called before the first frame update
    private void Start()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            ResourceDisplay display = Instantiate(ResourceDisplayPrefab, transform, false);
            display.Init(type, Simulation);
        }
    }
}
