using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private readonly Dictionary<ResourceType, Resource> resources = new Dictionary<ResourceType, Resource>();

    public Resource GetResource(ResourceType type)
    {
        return resources[type];
    }

    private void Start()
    {
        foreach (Resource resource in GetComponents<Resource>())
        {
            resources[resource.Type] = resource;
        }
    }
}
