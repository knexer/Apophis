using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeList : MonoBehaviour
{
    [SerializeField] private RectTransform UpgradeDisplayParent;
    [SerializeField] private GameObject UpgradeParent;
    [SerializeField] private UpgradeDisplay UpgradeDisplayPrefab;

    void Start()
    {
        foreach (Upgrade upgrade in UpgradeParent.GetComponentsInChildren<Upgrade>())
        {
            UpgradeDisplay upgradeDisplay = Instantiate(UpgradeDisplayPrefab, UpgradeDisplayParent, false);
            upgradeDisplay.Upgrade = upgrade;
        }
    }
}
