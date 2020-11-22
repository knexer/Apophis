using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Text Name;
    [SerializeField] private TMP_Text Description;
    [SerializeField] private TMP_Text Cost;
    [SerializeField] private Text TimeToGet;
    [SerializeField] private RectTransform EffectsContainer;
    [SerializeField] private TMP_Text EffectDescriptionPrefab;

    [HideInInspector] public SimulationManager Simulation;
    [HideInInspector] public Timeline Timeline;
    [HideInInspector] public Upgrade Upgrade;

    private void Start()
    {
        Name.text = Upgrade.Name;
        Description.text = Upgrade.Description;
        Cost.text = Upgrade.Cost.Select(cost => cost.Abs().ToString())
            .Aggregate((left, right) => $"{left}, {right}");
        IUpgradeEffect[] effects = Upgrade.GetComponents<IUpgradeEffect>();
        EffectsContainer.gameObject.SetActive(effects.Length > 0);
        foreach (IUpgradeEffect effect in effects)
        {
            TMP_Text effectDescription = Instantiate(EffectDescriptionPrefab, EffectsContainer, false);
            effectDescription.text = effect.Describe();
        }

        GetComponent<Button>().onClick.AddListener(BuyUpgrade);

        Simulation.OnSimChanged += UpdateDisplay;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        int? timeToGet = Simulation.GetTimeToPurchase(Upgrade);
        if (timeToGet == null)
        {
            TimeToGet.text = "ERR";
            TimeToGet.color = Color.red;
        }
        else
        {
            TimeToGet.text = timeToGet.ToString() + " cycles";
            if (timeToGet < 6) TimeToGet.color = Color.green;
            else if (timeToGet < 30) TimeToGet.color = Color.yellow;
            else TimeToGet.color = Color.Lerp(Color.red, Color.yellow, .5f);
        }

        GetComponent<Button>().interactable = timeToGet != null && !Simulation.IsLocked;
    }

    private void BuyUpgrade()
    {
        Simulation.BuyUpgrade(Upgrade);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Timeline.PreviewUpgrade(Upgrade);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Timeline.EndUpgradePreview();
    }
}
