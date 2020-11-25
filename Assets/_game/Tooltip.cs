using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    public void SetTarget(TooltipTarget tooltipTarget)
    {
        // Configure layout
        RectTransform canvasTransform = tooltipTarget.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        RectTransform tooltipTransform = GetComponent<RectTransform>();
        RectTransform targetTransform = tooltipTarget.GetComponent<RectTransform>();

        tooltipTransform.SetParent(canvasTransform, false);

        float xOffset = targetTransform.rect.xMin;
        if (targetTransform.position.x > canvasTransform.rect.width / 2)
        {
            tooltipTransform.pivot = new Vector2(1, tooltipTransform.pivot.y);
            xOffset = targetTransform.rect.xMax;
        }
        tooltipTransform.position = ((Vector2)targetTransform.position) + new Vector2(xOffset, targetTransform.rect.yMax);

        // Populate with data
        foreach (ITooltipEntry entry in tooltipTarget.entries)
        {
            entry.AddTooltipEntry(this);
        }
    }
}
