using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    [SerializeField] public string Name;

    [SerializeField] public string Description;

    [SerializeField] public Cost[] Cost;

    public void Apply(Simulation sim, ResourceManager resources)
    {
        foreach (IUpgradeEffect effect in GetComponents<IUpgradeEffect>())
        {
            effect.Apply(sim, resources);
        }
    }
}

[Serializable]
public class Cost
{
    public ResourceType Type;
    public int Amount;
}
