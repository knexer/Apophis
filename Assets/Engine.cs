using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour, IUpgradeEffect
{
    [SerializeField] private ResourceDelta OperatingCost;

    public string Describe()
    {
        return $"Consume {OperatingCost} to accelerate the asteroid, generating Deflection. Most effective when powered early.";
    }

    public void Apply(Simulation sim, ResourceManager resources)
    {
        if (resources.CanApplyNextCycle(OperatingCost))
        {
            resources.ApplyNextCycle(OperatingCost);
            resources.GetResource(ResourceType.Deflection).ChangeNextCycle += sim.MaxTime - sim.CurrentTime;
        }
    }
}
