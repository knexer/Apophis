using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Specialization : MonoBehaviour
{
    public string Name;

    public IEnumerable<Upgrade> Upgrades => GetComponentsInChildren<Upgrade>();
}
