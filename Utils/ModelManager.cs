using System;
using System.Collections.Generic;
using UnityEngine;
using ModelReplacement;

namespace LethalModelSwitcher.Utils;

public static class ModelManager
{
    public static Dictionary<string, List<ModelVariant>> RegisteredModels = new Dictionary<string, List<ModelVariant>>();

    public static void RegisterBaseModel(string modelName, Type modelType, AudioClip sound = null, GameObject modelPrefab = null)
    {
        if (!RegisteredModels.ContainsKey(modelName))
        {
            RegisteredModels[modelName] = new List<ModelVariant> { new ModelVariant(modelName, modelType, sound, modelPrefab, true) };
            ModelReplacementAPI.RegisterSuitModelReplacement(modelName, modelType);
            LethalModelSwitcher.Logger.LogInfo($"Registered base model: {modelName}");
        }
        else
        {
            LethalModelSwitcher.Logger.LogWarning($"Model {modelName} is already registered.");
        }
    }

    public static void RegisterModelVariant(string baseModelName, string variantName, Type variantType, AudioClip sound = null, GameObject modelPrefab = null)
    {
        if (RegisteredModels.ContainsKey(baseModelName))
        {
            RegisteredModels[baseModelName].Add(new ModelVariant(variantName, variantType, sound, modelPrefab, false));
            ModelReplacementAPI.RegisterModelReplacementException(variantType);
            LethalModelSwitcher.Logger.LogInfo($"Registered variant: {variantName} for base model: {baseModelName}");
        }
        else
        {
            LethalModelSwitcher.Logger.LogError($"Base model {baseModelName} not found. Register the base model first.");
        }
    }
}

public class ModelVariant
{
    public string Name { get; }
    public Type Type { get; }
    public AudioClip Sound { get; }
    public GameObject ModelPrefab { get; }
    public bool IsActive { get; private set; }

    public ModelVariant(string name, Type type, AudioClip sound, GameObject modelPrefab, bool isActive)
    {
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