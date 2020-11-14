using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDisplay : MonoBehaviour
{
    [SerializeField] private ResourceType Type;

    [SerializeField] private Text Name;
    [SerializeField] private Text Amount;
    [SerializeField] private Text Growth;

    public Simulation Simulation;
    private Resource resource;

    private void Start()
    {
        resource = FindObjectsOfType<Resource>().First(res => res.Type == Type);
        Name.text = Type.ToString();

        Simulation.OnSimChanged += UpdateResource;
        UpdateResource();
    }

    private void UpdateResource()
    {
        Amount.text = resource.Amount.ToString();
        Growth.text = $"+{resource.Growth}/cycle";
    }
}
