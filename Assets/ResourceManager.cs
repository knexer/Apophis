using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private readonly Dictionary<ResourceType, Resource> resources = new Dictionary<ResourceType, Resource>();

    public IEnumerable<Resource> Resources => resources.Values;

    public Resource GetResource(ResourceType type)
    {
        return resources[type];
    }

    private void Awake()
    {
        foreach (Resource resource in GetComponents<Resource>())
        {
            resources[resource.Type] = resource;
        }
    }

    public bool CanApplyNextCycle(ResourceDelta delta)
    {
        Resource resource = GetResource(delta.Type);
        return resource.Amount + resource.ChangeNextCycle + delta.Amount >= 0;
    }

    public bool CanApplyImmediate(ResourceDelta delta)
    {
        Resource resource = GetResource(delta.Type);
        return resource.Amount + delta.Amount >= 0;
    }

    public void ApplyNextCycle(ResourceDelta delta)
    {
        Resource resource = GetResource(delta.Type);
        resource.ChangeNextCycle += delta.Amount;
    }

    public void ApplyImmediate(ResourceDelta delta)
    {
        Resource resource = GetResource(delta.Type);
        resource.Amount += delta.Amount;
    }
}
