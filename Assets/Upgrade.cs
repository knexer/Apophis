using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    [SerializeField] public string Name;

    [SerializeField] public string Description;

    [SerializeField] public Cost[] Cost;

    public void OnBought(Simulation sim, ResourceManager resources)
    {
        resources.GetResource(ResourceType.Metal).Growth += 1;
    }
}

[Serializable]
public class Cost
{
    public ResourceType Type;
    public int Amount;
}
