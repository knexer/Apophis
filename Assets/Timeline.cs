using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timeline : MonoBehaviour
{
    [SerializeField] private Simulation sim;
    [SerializeField] private RectTransform Background;
    [SerializeField] private RectTransform Foreground;

    void Start()
    {
        Canvas.ForceUpdateCanvases();
        sim.OnSimChanged += UpdateTimeline;
        UpdateTimeline();
    }

    private void UpdateTimeline()
    {
        float nonForegroundWidth = Background.rect.width * (((float) sim.CurrentTime / sim.MaxTime) - 1);
        Debug.Log((float)sim.CurrentTime / sim.MaxTime);
        Debug.Log(Background.rect.width);
        Debug.Log(Foreground.sizeDelta);
        Foreground.sizeDelta = new Vector2(nonForegroundWidth, Foreground.sizeDelta.y);
    }
}
