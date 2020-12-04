using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceTypeConfig : MonoBehaviour
{
    public static Dictionary<ResourceType, ResourceTypeConfig> configs = new Dictionary<ResourceType, ResourceTypeConfig>();

    [SerializeField] public ResourceType Type;
    [SerializeField] public Sprite BigImage;
    [SerializeField] public Sprite Icon;
    [SerializeField] public string Name;
    [SerializeField] public string IconEmbedTag;
    public string NameAndIcon => $"{IconEmbedTag}\u00A0{Name}";

    private void Awake()
    {
        configs[Type] = this;
    }
}
