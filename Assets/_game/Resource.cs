using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public ResourceType Type;
    public int Amount;
    public int Growth; // TODO something more sophisticated to handle many kinds of modifiers to growth
}
public enum ResourceType
{
    Metal,
    Volatiles,
    Components,
    Fuel
}
