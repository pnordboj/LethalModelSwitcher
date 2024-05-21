using System;
using System.Collections.Generic;
using ModelReplacement;
using UnityEngine;

namespace LethalModelSwitcher.Utils
{
    public static class ModelManager
    {
        // Dictionary to store base models
        public static Dictionary<string, BaseModel> BaseModels = new Dictionary<string, BaseModel>();

        // Dictionary to store variants keyed by the base model name
        public static Dictionary<string, List<ModelVariant>> VariantModels = new Dictionary<string, List<ModelVariant>>();

        public static void RegisterBaseModel(string suitName, string modelName, Type modelType, AudioClip sound = null, GameObject modelPrefab = null)
        {
            if (!BaseModels.ContainsKey(suitName))
            {
                var baseModel = new BaseModel(suitName, modelName, modelType, sound, modelPrefab);
                BaseModels[suitName] = baseModel;
                VariantModels[suitName] = new List<ModelVariant>();
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
            if (BaseModels.ContainsKey(baseSuitName))
            {
                var variant = new ModelVariant(baseSuitName, variantName, variantType, sound, modelPrefab);
                VariantModels[baseSuitName].Add(variant);
                LethalModelSwitcher.Logger.LogInfo($"Registered variant: {variantName} for base suit: {baseSuitName}");
            }
            else
            {
                LethalModelSwitcher.Logger.LogError($"Base suit {baseSuitName} not found. Register the base model first.");
            }
        }

        public static string GetSuitName(int suitId)
        {
            foreach (var baseModel in BaseModels.Values)
            {
                if (baseModel.Type.GetHashCode() == suitId)
                {
                    return baseModel.SuitName;
                }

                if (VariantModels.ContainsKey(baseModel.SuitName))
                {
                    foreach (var variant in VariantModels[baseModel.SuitName])
                    {
                        if (variant.Type.GetHashCode() == suitId)
                        {
                            return baseModel.SuitName;
                        }
                    }
                }
            }
            LethalModelSwitcher.Logger.LogWarning($"No active suit found for suitId: {suitId}");
            return null;
        }

        public static List<ModelVariant> GetVariants(string suitName)
        {
            if (VariantModels.ContainsKey(suitName))
            {
                return VariantModels[suitName];
            }
            LethalModelSwitcher.Logger.LogError($"Variants for suit {suitName} not found.");
            return null;
        }

        public static BaseModel GetBaseModel(string suitName)
        {
            if (BaseModels.ContainsKey(suitName))
            {
                return BaseModels[suitName];
            }
            LethalModelSwitcher.Logger.LogError($"Base model for suit {suitName} not found.");
            return null;
        }

        public static void SetModelActive(string suitName, string modelName)
        {
            if (BaseModels.ContainsKey(suitName) && (BaseModels[suitName].Name == modelName))
            {
                BaseModels[suitName].SetActive(true);
                foreach (var variant in VariantModels[suitName])
                {
                    variant.SetActive(false);
                }
                LethalModelSwitcher.Logger.LogInfo($"Set base model {modelName} as active for suit {suitName}");
                return;
            }

            if (VariantModels.ContainsKey(suitName))
            {
                foreach (var variant in VariantModels[suitName])
                {
                    variant.SetActive(variant.Name == modelName);
                }
                BaseModels[suitName].SetActive(false);
                LethalModelSwitcher.Logger.LogInfo($"Set variant model {modelName} as active for suit {suitName}");
            }
            else
            {
                LethalModelSwitcher.Logger.LogError($"Suit {suitName} not found. Cannot set model {modelName} as active.");
            }
        }
    }

    public class BaseModel
    {
        public string SuitName { get; }
        public string Name { get; }
        public Type Type { get; }
        public AudioClip Sound { get; }
        public GameObject ModelPrefab { get; }
        public bool IsActive { get; private set; }

        public BaseModel(string suitName, string name, Type type, AudioClip sound, GameObject modelPrefab)
        {
            SuitName = suitName;
            Name = name;
            Type = type;
            Sound = sound;
            ModelPrefab = modelPrefab;
            IsActive = false;
        }

        public void SetActive(bool isActive)
        {
            IsActive = isActive;
        }
    }

    public class ModelVariant
    {
        public string BaseSuitName { get; }
        public string Name { get; }
        public Type Type { get; }
        public AudioClip Sound { get; }
        public GameObject ModelPrefab { get; }
        public bool IsActive { get; private set; }

        public ModelVariant(string baseSuitName, string name, Type type, AudioClip sound, GameObject modelPrefab)
        {
            BaseSuitName = baseSuitName;
            Name = name;
            Type = type;
            Sound = sound;
            ModelPrefab = modelPrefab;
            IsActive = false;
        }

        public void SetActive(bool isActive)
        {
            IsActive = isActive;
        }
    }
}
