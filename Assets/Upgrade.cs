using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    [SerializeField] public string Name;

    [SerializeField] public string Description;

    [SerializeField] public Cost[] Cost;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[Serializable]
public class Cost
{
    public ResourceType Type;
    public int Amount;
}
