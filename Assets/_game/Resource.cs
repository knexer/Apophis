using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[Serializable]
public class Resource : ITooltipEntry
{
    public ResourceType Type { get; private set; }
    public int Amount;
    public int ChangeNextCycle;

    public Resource(ResourceType type)
    {
        Type = type;
    }

    public void AddTooltipEntry(Tooltip host)
    {
        TMP_Text text = UnityEngine.Object.Instantiate(ResourceTooltipConfig.Instance.ResourceTooltipPrefab, host.transform, false);
        text.text = $"{Amount} {ResourceTooltipConfig.Instance.GetComponentsInChildren<ResourceTypeConfig>().First(config => config.Type == Type).NameAndIcon}";
    }
}
public enum ResourceType
{
    Metal,
    Volatiles,
    Parts,
    Fuel,
    Capacity,
    Deflection
}
