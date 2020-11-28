using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TimelineElement : MonoBehaviour
{
    [SerializeField] private Color FutureColor;
    [SerializeField] private Color PastColor;
    [SerializeField] private Color UpgradeColor;
    [SerializeField] private Color PreviewColor;
    [SerializeField] private Color UpgradePreviewColor;
    [SerializeField] private Color InvalidPreviewColor;

    [SerializeField] private Image Background;

    private SimulationHistory sim;
    private int timeStep;

    public void Init(SimulationHistory sim, int timeStep)
    {
        this.sim = sim;
        this.timeStep = timeStep;
        GetComponent<TooltipTarget>().entrySource = GetTooltipEntries;
        Layout();
        Paint();
    }

    public void Layout()
    {
        float totalWidth = transform.parent.GetComponent<RectTransform>().rect.width;
        float widthPerElement = totalWidth / sim.ResourcesAtEachTime.Count;
        float leftX = widthPerElement * timeStep;
        RectTransform rect = GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(leftX, 0);
        rect.sizeDelta = new Vector2(widthPerElement, rect.sizeDelta.y);
    }

    public void Paint()
    {
        if (sim.CurrentTimeSim.CurrentTime >= timeStep)
        {
            Background.color = PastColor;
        }
        else
        {
            Background.color = FutureColor;
        }

        for (int i = 0; i < sim.CurrentTimeSim.boughtUpgradeTimes.Count; i++)
        {
            if (sim.CurrentTimeSim.boughtUpgradeTimes[i] == timeStep)
            {
                Background.color = UpgradeColor;
            }
            if (sim.CurrentTimeSim.boughtUpgradeTimes[i] >= timeStep)
            {
                break;
            }
        }
    }

    private List<ITooltipEntry> GetTooltipEntries()
    {
        List<ITooltipEntry> entries = new List<ITooltipEntry>();
        for (int i = 0; i < sim.CurrentTimeSim.boughtUpgradeTimes.Count; i++)
        {
            if (sim.CurrentTimeSim.boughtUpgradeTimes[i] == timeStep)
            {
                entries.Add(sim.CurrentTimeSim.boughtUpgrades[i]);
            }
        }
        entries.AddRange(sim.ResourcesAtEachTime[timeStep].Resources);

        return entries;
    }

    public void PaintPreview()
    {
        Background.color = PreviewColor;
    }

    public void PaintUpgradePreview()
    {
        Background.color = UpgradePreviewColor;
    }

    internal void PaintInvalidPreview()
    {
        Background.color = InvalidPreviewColor;
    }
}
