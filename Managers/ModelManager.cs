﻿using System;
using System.Collections.Generic;
using ModelReplacement;
using UnityEngine;

namespace LethalModelSwitcher.Utils;

public static class ModelManager
{
    public static Dictionary<string, List<ModelVariant>> RegisteredModels = new Dictionary<string, List<ModelVariant>>();

    public static void RegisterBaseModel(string suitName, string modelName, Type modelType, AudioClip sound = null, GameObject modelPrefab = null)
    {
        if (!RegisteredModels.ContainsKey(suitName))
        {
            RegisteredModels[suitName] = new List<ModelVariant> { new ModelVariant(suitName, modelName, modelType, sound, modelPrefab, true) };
            ModelReplacementAPI.RegisterSuitModelReplacement(suitName, modelType);
            LethalModelSwitcher.Logger.LogInfo($"Registered base model: {modelName} for suit: {suitName}");
        }
        else
        {
            LethalModelSwitcher.Logger.LogWarning($"Suit {suitName} already has a registered model.");
        }
    }

    public static void RegisterModelVariant(string baseSuitName, string variantName, Type variantType, AudioClip sound = null, GameObject modelPrefab = null)
    {
        if (RegisteredModels.ContainsKey(baseSuitName))
        {
            RegisteredModels[baseSuitName].Add(new ModelVariant(baseSuitName, variantName, variantType, sound, modelPrefab, false));
            LethalModelSwitcher.Logger.LogInfo($"Registered variant: {variantName} for base suit: {baseSuitName}");
        }
        else
        {
            LethalModelSwitcher.Logger.LogError($"Base suit {baseSuitName} not found. Register the base model first.");
        }
    }

    public static string GetSuitName(int suitId)
    {
        return "Suit" + suitId;      
    }
}

public class ModelVariant
{
    public string SuitName { get; }
    public string Name { get; }
    public Type Type { get; }
    public AudioClip Sound { get; }
    public GameObject ModelPrefab { get; }
    public bool IsActive { get; private set; }

    public ModelVariant(string suitName, string name, Type type, AudioClip sound, GameObject modelPrefab, bool isActive)
    {
        SuitName = suitName;
        Name = name;
        Type = type;
        Sound = sound;
        ModelPrefab = modelPrefab;
        IsActive = isActive;
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
    }
}