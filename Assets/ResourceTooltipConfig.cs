using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceTooltipConfig : MonoBehaviour
{
    [SerializeField] public TMP_Text ResourceTooltipPrefab;

    public static ResourceTooltipConfig Instance;

    void Awake()
    {
        Instance = this;
    }
}
