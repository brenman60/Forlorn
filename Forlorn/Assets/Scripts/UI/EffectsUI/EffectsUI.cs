using System.Collections.Generic;
using UnityEngine;

public class EffectsUI : MonoBehaviour
{
    [SerializeField] private GameObject effectTemplate;
    [SerializeField] private Transform effectList;

    private List<EffectUI> effectUIs = new List<EffectUI>();

    private void Start()
    {
        foreach (Effect currentEffect in RunManager.Instance.statManager.effects)
            EffectsChanged(currentEffect, true);

        StatManager.effectsChanged += EffectsChanged;
    }

    private void OnDestroy()
    {
        StatManager.effectsChanged -= EffectsChanged;
    }

    private void EffectsChanged(Effect effect, bool added)
    {
        if (!effect.showsIcon) return;

        if (added)
        {
            GameObject effectUIObj = Instantiate(effectTemplate, effectList);
            EffectUI effectUI = effectUIObj.GetComponent<EffectUI>();
            effectUI.effect = effect;

            effectUIs.Add(effectUI);
            effectUIObj.SetActive(true);
        }
        else
        {
            foreach (EffectUI effectUI in effectUIs.ToArray())
                if (effectUI.effect == effect)
                {
                    effectUIs.Remove(effectUI);
                    Destroy(effectUI.gameObject);
                }
        }
    }
}
