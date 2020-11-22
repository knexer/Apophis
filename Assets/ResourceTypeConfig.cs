using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceTypeConfig : MonoBehaviour
{
    [SerializeField] public ResourceType Type;
    [SerializeField] public Sprite BigImage;
    [SerializeField] public Sprite Icon;
    [SerializeField] public string Name;
    [SerializeField] public string IconEmbedTag;
    public string NameAndIcon => $"{IconEmbedTag} {Name}";
}
