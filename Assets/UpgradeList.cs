using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeList : MonoBehaviour
{
    [SerializeField] private RectTransform SpecializationDisplayParent;
    [SerializeField] private GameObject SpecializationsContainer;
    [SerializeField] private SpecializationDisplay SpecializationDisplayPrefab;
    [SerializeField] private SimulationManager Simulation;
    [SerializeField] private ParallelTimelineDisplay Timelines;

    void Start()
    {
        foreach (Specialization spec in SpecializationsContainer.GetComponentsInChildren<Specialization>())
        {
            SpecializationDisplay specDisplay = Instantiate(SpecializationDisplayPrefab, SpecializationDisplayParent, false);
            specDisplay.Init(spec, Simulation, Timelines);
        }
    }
}
