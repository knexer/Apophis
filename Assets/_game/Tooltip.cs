using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    private void Awake()
    {
        // TODO dis suc
        transform.SetParent(FindObjectOfType<Canvas>().transform, false);
    }
    // Update is called once per frame
    private void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void SetTarget(TooltipTarget tooltipTarget)
    {
        // Populate UI
        foreach (ITooltipEntry entry in tooltipTarget.entries)
        {
            entry.AddTooltipEntry(this);
        }
        Update();
    }
}
