using System;
using System.Collections.Generic;
using ModelReplacement;
using UnityEngine;

namespace LethalModelSwitcher.Utils
{
    public static class ModelManager
    {
        public static Dictionary<string, BaseModel> BaseModels = new Dictionary<string, BaseModel>();
        public static Dictionary<string, List<ModelVariant>> VariantModels = new Dictionary<string, List<ModelVariant>>();

        public static void RegisterBaseModel(string suitName, string modelName, Type modelType, AudioClip sound = null, GameObject modelPrefab = null)
        {
            if (string.IsNullOrEmpty(suitName))
            {
                LethalModelSwitcher.Logger.LogError("RegisterBaseModel: suitName is null or empty.");
                return;
            }

            if (!BaseModels.ContainsKey(suitName))
            {
                BaseModels[suitName] = new BaseModel(suitName, modelName, modelType, sound, modelPrefab);
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
            if (string.IsNullOrEmpty(baseSuitName))
            {
                LethalModelSwitcher.Logger.LogError("RegisterModelVariant: baseSuitName is null or empty.");
                return;
            }

            if (BaseModels.ContainsKey(baseSuitName))
            {
                if (!VariantModels.ContainsKey(baseSuitName))
                {
                    VariantModels[baseSuitName] = new List<ModelVariant>();
                }
                ModelReplacementAPI.RegisterModelReplacementException(variantType);
                VariantModels[baseSuitName].Add(new ModelVariant(baseSuitName, variantName, variantType, sound, modelPrefab));
                LethalModelSwitcher.Logger.LogInfo($"Registered variant: {variantName} for base suit: {baseSuitName}");
            }
            else
            {
                LethalModelSwitcher.Logger.LogError($"Base suit {baseSuitName} not found. Register the base model first.");
            }
        }

        public static string GetSuitName(int suitId)
        {
            foreach (var suitEntry in BaseModels)
            {
                if (suitEntry.Value.Type.GetHashCode() == suitId)
                {
                    return suitEntry.Key;
                }
            }
            LethalModelSwitcher.Logger.LogWarning($"No active suit found for suitId: {suitId}");
            return null;
        }

        public static List<ModelVariant> GetVariants(string suitName)
        {
            if (string.IsNullOrEmpty(suitName))
            {
                LethalModelSwitcher.Logger.LogError("GetVariants: suitName is null or empty.");
                return null;
            }

            if (VariantModels.ContainsKey(suitName))
            {
                return VariantModels[suitName];
            }
            LethalModelSwitcher.Logger.LogError($"Suit {suitName} not found. Cannot get variants.");
            return null;
        }

        public static BaseModel GetBaseModel(string suitName)
        {
            if (string.IsNullOrEmpty(suitName))
            {
                LethalModelSwitcher.Logger.LogError("GetBaseModel: suitName is null or empty.");
                return null;
            }

            if (BaseModels.ContainsKey(suitName))
            {
                return BaseModels[suitName];
            }
            LethalModelSwitcher.Logger.LogError($"Suit {suitName} not found. Cannot get base model.");
            return null;
        }

        public static void SetModelActive(string suitName, string modelName)
        {
            if (string.IsNullOrEmpty(suitName))
            {
                LethalModelSwitcher.Logger.LogError("SetModelActive: suitName is null or empty.");
                return;
            }

            if (BaseModels.ContainsKey(suitName))
            {
                var baseModel = BaseModels[suitName];
                var variants = VariantModels.ContainsKey(suitName) ? VariantModels[suitName] : new List<ModelVariant>();

                foreach (var variant in variants)
                {
                    variant.SetActive(variant.Name == modelName);
                }
                baseModel.SetActive(baseModel.Name == modelName);

                LethalModelSwitcher.Logger.LogInfo($"Set model {modelName} as active for suit {suitName}");
            }
            else
            {
                LethalModelSwitcher.Logger.LogError($"Suit {suitName} not found. Cannot set model {modelName} as active.");
            }
        }
    }

    public abstract class ModelBase
    {
        public string SuitName { get; }
        public string Name { get; }
        public Type Type { get; }
        public AudioClip Sound { get; }
        public GameObject ModelPrefab { get; }
        public bool IsActive { get; private set; }

        protected ModelBase(string suitName, string name, Type type, AudioClip sound, GameObject modelPrefab)
        {
            SuitName = suitName;
            Name = name;
            Type = type;
            Sound = sound;
            ModelPrefab = modelPrefab;
        }

        public void SetActive(bool isActive)
        {
            IsActive = isActive;
        }
    }

    public class BaseModel : ModelBase
    {
        public BaseModel(string suitName, string name, Type type, AudioClip sound, GameObject modelPrefab)
            : base(suitName, name, type, sound, modelPrefab) { }
    }

    public class ModelVariant : ModelBase
    {
        public ModelVariant(string suitName, string name, Type type, AudioClip sound, GameObject modelPrefab)
            : base(suitName, name, type, sound, modelPrefab) { }
    }
}
