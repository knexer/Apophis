using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Tooltip TooltipPrefab;

    private Tooltip Tooltip;

    public List<ITooltipEntry> entries = new List<ITooltipEntry>();

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (entries.Count > 0)
        {
            Tooltip = Instantiate(TooltipPrefab);
            Tooltip.SetTarget(this);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Tooltip != null)
            Destroy(Tooltip.gameObject);
    }

    private void OnDestroy()
    {
        if (Tooltip != null)
            Destroy(Tooltip.gameObject);
    }
}
