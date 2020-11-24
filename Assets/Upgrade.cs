using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Upgrade : MonoBehaviour, ITooltipEntry
{
    [SerializeField] private RectTransform TooltipEntryPrefab;
    [SerializeField] public string Name;

    [SerializeField] public string Description;

    [SerializeField] public ResourceDelta[] Cost;

    private IUpgradeEffect[] effects;

    private void Awake()
    {
        effects = GetComponents<IUpgradeEffect>();
    }

    public void Apply(Simulation sim, ResourceManager resources)
    {
        foreach (IUpgradeEffect effect in effects)
        {
            effect.Apply(sim, resources);
        }
    }

    public void AddTooltipEntry(Tooltip host)
    {
        Instantiate(TooltipEntryPrefab, host.transform, false).GetComponentInChildren<TMP_Text>().text = Name;
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
        return $"{Amount} {ResourceTypeConfig.configs[Type].NameAndIcon}";
    }
}
