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
    [SerializeField] private Text Quantity;
    [SerializeField] private RectTransform EffectsContainer;
    [SerializeField] private TMP_Text EffectDescriptionPrefab;
    [SerializeField] private Button BuyButton;

    [HideInInspector] public SimulationManager Simulation;
    [HideInInspector] public ParallelTimelineDisplay Timelines;
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

        BuyButton.onClick.AddListener(BuyUpgrade);

        Simulation.OnSimChanged += UpdateDisplay;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        Quantity.text = $"{Simulation.ActualSims.CurrentSim.boughtUpgrades.Count(upgrade => upgrade == Upgrade)} owned";

        int? timeToGet = Simulation.GetTimeToPurchase(Upgrade);
        BuyButton.interactable = timeToGet != null && !Simulation.IsLocked;
        BuyButton.GetComponent<UpgradeBuyButton>().UpdateButton(timeToGet);
    }

    private void BuyUpgrade()
    {
        Simulation.BuyUpgrade(Upgrade);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Timelines.CurrentTimeline.PreviewUpgrade(Upgrade);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Timelines.CurrentTimeline.EndUpgradePreview();
    }
}
