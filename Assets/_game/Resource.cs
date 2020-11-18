using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public ResourceType Type;
    public int Amount;
    public int ChangeNextCycle;
}
public enum ResourceType
{
    Metal,
    Volatiles,
    Components,
    Fuel,
    Capacity,
    Deflection
}
