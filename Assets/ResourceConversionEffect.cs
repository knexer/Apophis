using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.AccessControl;
using UnityEngine;

public class ResourceConversionEffect : MonoBehaviour, IUpgradeEffect
{
    [SerializeField] private ResourceDelta[] Effects;
    public string Describe()
    {
        string costs = Effects.Where(effect => effect.Amount < 0).Select(effect => effect.Abs().ToString())
            .Aggregate((left, right) => $"{left} and {right}");
        string results = Effects.Where(effect => effect.Amount > 0).Select(effect => effect.ToString())
            .Aggregate((left, right) => $"{left} and {right}");
        return $"Each cycle, convert {costs} into {results}.";
    }

    public void Apply(Simulation sim, ResourceManager resources)
    {
        // Don't convert if some input is not present in sufficient quantity.
        if (!Effects.All(resources.CanApplyNextCycle))
            return;
        foreach (ResourceDelta effect in Effects)
        {
            resources.ApplyNextCycle(effect);
        }
    }
}
