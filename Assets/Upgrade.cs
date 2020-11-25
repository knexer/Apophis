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

    public void Apply(Simulation sim, ResourceCollection resources)
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
