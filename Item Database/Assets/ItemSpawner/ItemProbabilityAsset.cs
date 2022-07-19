using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemProbabilityAsset")]
public class ItemProbabilityAsset : ScriptableObject
{
    [Header("Label Settings")]
    [Space(10)]

    public List<WeightedLabel> labels;
    public AssetLabelReference defaultLabel;

    public string GetRandomLabel()
    {
        int totalWeight = 0;

        foreach (var l in labels)
        {
            totalWeight += l.weight;
        }

        int r = UnityEngine.Random.Range(0, totalWeight + 1);
        int currentWeight = 0;

        foreach (WeightedLabel l in labels)
        {
            if (r <= currentWeight + l.weight)
            {
                return l.label.labelString;
            }
            else
            {
                currentWeight += l.weight;
            }
        }

        Debug.LogWarning($"{name}: no weighted choice selected, using default.");

        return defaultLabel.labelString;

    }

    [Serializable]
    public class WeightedLabel
    {
        public AssetLabelReference label;
        public int weight;
    }

}
