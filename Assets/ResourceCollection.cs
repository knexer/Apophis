using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ResourceCollection
{
    [SerializeField]
    private List<Resource> resources;

    public IEnumerable<Resource> Resources => resources;

    public ResourceCollection()
    {
        resources = new List<Resource>();
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            resources.Add(new Resource(type));
        }
    }

    public ResourceCollection(ResourceCollection other) : this()
    {
        foreach (Resource resource in Resources)
        {
            Resource otherResource = other.GetResource(resource.Type);
            resource.Amount = otherResource.Amount;
            resource.ChangeNextCycle = otherResource.ChangeNextCycle;
        }
    }

    public Resource GetResource(ResourceType type)
    {
        return resources.First(resource => resource.Type == type);
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

[Serializable]
public class ResourceDelta
{
    public ResourceType Type;
    public int Amount;

    public ResourceDelta Abs() => new ResourceDelta() { Type = Type, Amount = Mathf.Abs(Amount) };

    public override string ToString()
    {
        return $"{Amount} {ResourceTypeConfig.configs[Type].NameAndIcon}";
    }
}
