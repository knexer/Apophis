using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallelTimelineDisplay : MonoBehaviour
{
    [SerializeField] private Timeline TimelinePrefab;
    [SerializeField] private RectTransform CurrentTimelineContainer;
    [SerializeField] private RectTransform ParallelTimelinesContainer;
    [SerializeField] private SimulationManager sim;

    int previousNumParallelTimelines;

    public Timeline CurrentTimeline;

    // Start is called before the first frame update
    void Start()
    {
        CurrentTimeline = Instantiate(TimelinePrefab, CurrentTimelineContainer, false);
        CurrentTimeline.Init(sim, null);
        previousNumParallelTimelines = 0;
        sim.OnSimChanged += MaybeAddTimeline;
    }

    private void MaybeAddTimeline()
    {
        if (previousNumParallelTimelines < sim.OtherTimelineHistories.Count)
        {
            Instantiate(TimelinePrefab, ParallelTimelinesContainer, false).Init(sim, previousNumParallelTimelines);
            previousNumParallelTimelines++;
        }
    }
}
