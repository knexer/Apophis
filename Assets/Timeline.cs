using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timeline : MonoBehaviour
{
    [SerializeField] private Simulation sim;
    [SerializeField] private RectTransform Background;
    [SerializeField] private RectTransform Foreground;
    [SerializeField] private RectTransform Preview;

    private int numPreviews = 0;
    private int? lastPreviewTime = 0;

    void Start()
    {
        Canvas.ForceUpdateCanvases();
        sim.OnSimChanged += UpdateTimeline;
        UpdateTimeline();
    }

    private void UpdateTimeline()
    {
        UpdateTimelineElement(Foreground, sim.CurrentTime);
    }

    public void PreviewUpgrade(Upgrade upgrade)
    {
        int? previewTime = sim.GetTimeToPurchase(upgrade);
        if (numPreviews > 0 && lastPreviewTime != previewTime)
        {
            throw new Exception($"Trying to preview time {previewTime} while already showing a preview of time {lastPreviewTime}");
        }

        numPreviews++;
        lastPreviewTime = previewTime;

        // TODO show a different color/size if null
        UpdateTimelineElement(Preview, sim.CurrentTime + previewTime ?? 0);
    }

    public void EndUpgradePreview()
    {
        if (numPreviews <= 0)
        {
            throw new Exception("Ending a preview twice, apparently!");
        }

        numPreviews--;

        if (numPreviews == 0)
        {
            UpdateTimelineElement(Preview, 0);
        }
    }

    private void UpdateTimelineElement(RectTransform element, int timeToShow)
    {
        float negativeWidth = Background.rect.width * (((float)timeToShow / sim.MaxTime) - 1);
        element.sizeDelta = new Vector2(negativeWidth, element.sizeDelta.y);
    }
}
