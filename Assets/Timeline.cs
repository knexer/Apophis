using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timeline : MonoBehaviour
{
    [SerializeField] private SimulationManager sim;
    [SerializeField] private RectTransform Background;
    [SerializeField] private RectTransform Foreground;
    [SerializeField] private RectTransform Preview;
    [SerializeField] private RectTransform UpgradeBuiltBoxPrefab;

    private int numPreviews = 0;
    private int? lastPreviewTime = 0;

    private List<RectTransform> UpgradeIndicatorBoxes = new List<RectTransform>();

    void Start()
    {
        Canvas.ForceUpdateCanvases();
        sim.OnSimChanged += UpdateTimeline;
        UpdateTimeline();
        UpdateTimelineElement(Preview, 0);
    }

    private void UpdateTimeline()
    {
        foreach (RectTransform box in UpgradeIndicatorBoxes)
        {
            Destroy(box.gameObject);
        }
        UpgradeIndicatorBoxes.Clear();
        UpdateTimelineElement(Foreground, sim.ActualSims.CurrentSim.CurrentTime);
        for (int i = 0; i < sim.ActualSims.CurrentSim.boughtUpgrades.Count; i++)
        {
            RectTransform indicator = Instantiate(UpgradeBuiltBoxPrefab, transform, false);
            int timeStepBuilt = sim.ActualSims.CurrentSim.boughtUpgradeTimes[i];
            indicator.anchoredPosition = new Vector2(XPositionForTimestep(timeStepBuilt), 0);
            UpgradeIndicatorBoxes.Add(indicator);
        }
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
        UpdateTimelineElement(Preview, sim.ActualSims.CurrentSim.CurrentTime + previewTime ?? 0);
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

    private float XPositionForTimestep(int timeToShow)
    {
        return Background.rect.width * ((float)timeToShow / sim.ActualSims.CurrentSim.MaxTime);
    }

    private void UpdateTimelineElement(RectTransform element, int timeToShow)
    {
        element.sizeDelta = new Vector2(XPositionForTimestep(timeToShow), element.sizeDelta.y);
    }
}
