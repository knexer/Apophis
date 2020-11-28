using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Timeline : UIBehaviour
{
    [SerializeField] private TimelineElement timelineElementPrefab;

    private SimulationManager simManager;
    private int? timelineIndex;

    private int numPreviews = 0;
    private int? lastPreviewTime = 0;
    private Simulation lastSim;

    private List<TimelineElement> Elements = new List<TimelineElement>();

    private Simulation CurrentSim => timelineIndex.HasValue ? simManager.ActualSims.PreviousSims[timelineIndex.Value] : simManager.ActualSims.CurrentSim;
    private SimulationHistory CurrentHistory => timelineIndex.HasValue ? simManager.OtherTimelineHistories[timelineIndex.Value] : simManager.CurrentTimelineHistory;

    public void Init(SimulationManager simManager, int? timelineIndex)
    {
        this.simManager = simManager;
        this.timelineIndex = timelineIndex;

        this.simManager.OnSimChanged += UpdateTimeline;
        UpdateTimeline();
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        foreach (TimelineElement element in Elements)
        {
            element.Layout();
        }
    }

    private void UpdateTimeline()
    {
        if (lastSim != CurrentSim)
        {
            lastSim = CurrentSim;
            RebuildTimeline();
        }
        RepaintTimeline();
    }

    private void RebuildTimeline()
    {
        foreach (TimelineElement element in Elements)
        {
            Destroy(element.gameObject);
        }
        Elements.Clear();

        for (int timeStep = 0; timeStep <= CurrentSim.MaxTime; timeStep++)
        {
            TimelineElement element = Instantiate(timelineElementPrefab, transform, false);
            element.Init(CurrentHistory, timeStep);
            Elements.Add(element);
        }
    }

    private void RepaintTimeline()
    {
        foreach (TimelineElement element in Elements)
        {
            element.Paint();
        }
    }

    public void PreviewUpgrade(Upgrade upgrade)
    {
        int? previewTime = simManager.GetTimeToPurchase(upgrade);
        if (numPreviews > 0 && lastPreviewTime != previewTime)
        {
            throw new Exception($"Trying to preview time {previewTime} while already showing a preview of time {lastPreviewTime}");
        }

        numPreviews++;
        lastPreviewTime = previewTime;

        RepaintTimeline();
        if (lastPreviewTime != null)
        {
            int currentTime = CurrentSim.CurrentTime;
            for (int timeStep = currentTime + 1; timeStep < currentTime + lastPreviewTime; timeStep++)
            {
                Elements[timeStep].PaintPreview();
            }

            Elements[currentTime + lastPreviewTime.Value].PaintUpgradePreview();
        }
        else
        {
            int currentTime = CurrentSim.CurrentTime;
            for (int timeStep = currentTime + 1; timeStep < Elements.Count; timeStep++)
            {
                Elements[timeStep].PaintInvalidPreview();
            }
        }

        // TODO show a different color/size if null
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
            RepaintTimeline();
        }
    }
}
