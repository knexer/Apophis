using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    [SerializeField] public string Name;

    [SerializeField] public string Description;

    [SerializeField] public ResourceDelta[] Cost;

    public void Apply(Simulation sim, ResourceManager resources)
    {
        foreach (IUpgradeEffect effect in GetComponents<IUpgradeEffect>())
        {
            effect.Apply(sim, resources);
        }
    }
}

[Serializable]
public class ResourceDelta
{
    public ResourceType Type;
    public int Amount;

    public ResourceDelta Abs() => new ResourceDelta() {Type = Type, Amount = Mathf.Abs(Amount)};

    public override string ToString()
    {
        return $"{Amount} {Type}";
    }
}
