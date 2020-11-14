using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpgradeEffect
{
    string Describe();
    void Apply(Simulation sim, ResourceManager resources);
}
