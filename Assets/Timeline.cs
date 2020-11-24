using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timeline : MonoBehaviour
{
    [SerializeField] private SimulationManager sim;
    [SerializeField] private TimelineElement timelineElementPrefab;
    [SerializeField] private RectTransform UpgradeBuiltBoxPrefab;

    private int numPreviews = 0;
    private int? lastPreviewTime = 0;
    private Simulation lastCurrentSim;

    private List<TimelineElement> Elements = new List<TimelineElement>();

    void Start()
    {
        Canvas.ForceUpdateCanvases();
        sim.OnSimChanged += UpdateTimeline;
        UpdateTimeline();
    }

    private void UpdateTimeline()
    {
        if (lastCurrentSim != sim.ActualSims.CurrentSim)
        {
            lastCurrentSim = sim.ActualSims.CurrentSim;
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

        for (int timeStep = 0; timeStep <= sim.ActualSims.CurrentSim.MaxTime; timeStep++)
        {
            TimelineElement element = Instantiate(timelineElementPrefab, transform, false);
            element.Init(sim.ActualSims.CurrentSim, timeStep);
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
        int? previewTime = sim.GetTimeToPurchase(upgrade);
        if (numPreviews > 0 && lastPreviewTime != previewTime)
        {
            throw new Exception($"Trying to preview time {previewTime} while already showing a preview of time {lastPreviewTime}");
        }

        numPreviews++;
        lastPreviewTime = previewTime;

        RepaintTimeline();
        if (lastPreviewTime != null)
        {
            int currentTime = sim.ActualSims.CurrentSim.CurrentTime;
            for (int timeStep = currentTime + 1; timeStep < currentTime + lastPreviewTime; timeStep++)
            {
                Elements[timeStep].PaintPreview();
            }

            Elements[currentTime + lastPreviewTime.Value].PaintUpgradePreview();
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
