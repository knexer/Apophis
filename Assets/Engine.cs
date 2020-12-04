using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour, IUpgradeEffect
{
    [SerializeField] private ResourceDelta OperatingCost;

    public string Describe()
    {
        return $"Consume {OperatingCost.Abs()} to accelerate the asteroid, generating " +
            $"1 {ResourceTypeConfig.configs[ResourceType.Deflection].NameAndIcon} for each remaining cycle.";
    }

    public void Apply(Simulation sim, ResourceCollection resources)
    {
        if (resources.CanApplyNextCycle(OperatingCost))
        {
            resources.ApplyNextCycle(OperatingCost);
            resources.GetResource(ResourceType.Deflection).ChangeNextCycle += sim.MaxTime - sim.CurrentTime;
        }
    }
}
