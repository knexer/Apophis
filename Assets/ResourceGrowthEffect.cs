using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGrowthEffect : MonoBehaviour, IUpgradeEffect
{
    [SerializeField] private ResourceType Type;
    [SerializeField] private int Growth;

    public string Describe()
    {
        return $"Produces +{Growth} {ResourceTypeConfig.configs[Type].NameAndIcon}/cycle.";
    }

    public void Apply(Simulation sim, ResourceCollection resources)
    {
        resources.GetResource(Type).ChangeNextCycle += Growth;
    }
}
