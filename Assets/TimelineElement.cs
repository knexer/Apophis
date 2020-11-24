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

    [SerializeField] private Image Background;

    private Simulation sim;
    private int timeStep;

    public void Init(Simulation sim, int timeStep)
    {
        this.sim = sim;
        this.timeStep = timeStep;
        Paint();
    }

    public void Paint()
    {
        if (sim.CurrentTime >= timeStep)
        {
            Background.color = PastColor;
        }
        else
        {
            Background.color = FutureColor;
        }

        if (sim.boughtUpgradeTimes.Contains(timeStep))
        {
            Background.color = UpgradeColor;
        }
    }

    public void PaintPreview()
    {
        Background.color = PreviewColor;
    }

    public void PaintUpgradePreview()
    {
        Background.color = UpgradePreviewColor;
    }
}
