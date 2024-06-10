using System;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
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
            if (string.IsNullOrEmpty(suitName)) return;

            if (!BaseModels.ContainsKey(suitName))
            {
                BaseModels[suitName] = new BaseModel(suitName, modelName, modelType, sound, modelPrefab);
                ModelReplacementAPI.RegisterSuitModelReplacement(suitName, modelType);
            }
        }

        public static void RegisterModelVariant(string baseSuitName, string variantName, Type variantType, AudioClip sound = null, GameObject modelPrefab = null)
        {
            if (string.IsNullOrEmpty(baseSuitName)) return;

            if (!BaseModels.ContainsKey(baseSuitName)) return;

            if (!VariantModels.ContainsKey(baseSuitName))
            {
                VariantModels[baseSuitName] = new List<ModelVariant>();
            }
            ModelReplacementAPI.RegisterModelReplacementException(variantType);
            VariantModels[baseSuitName].Add(new ModelVariant(baseSuitName, variantName, variantType, sound, modelPrefab));
        }

        public static string GetSuitName(int suitId)
        {
            return BaseModels.FirstOrDefault(s => s.Value.Type.GetHashCode() == suitId).Key;
        }

        public static List<ModelVariant> GetVariants(string suitName)
        {
            if (string.IsNullOrEmpty(suitName)) return null;
            return VariantModels.TryGetValue(suitName, out var variants) ? variants : null;
        }

        public static BaseModel GetBaseModel(string suitName)
        {
            if (string.IsNullOrEmpty(suitName)) return null;
            return BaseModels.TryGetValue(suitName, out var baseModel) ? baseModel : null;
        }

        public static void SetModelActive(string suitName, string modelName)
        {
            if (string.IsNullOrEmpty(suitName)) return;

            if (BaseModels.TryGetValue(suitName, out var baseModel))
            {
                var variants = VariantModels.TryGetValue(suitName, out var variantList) ? variantList : new List<ModelVariant>();

                foreach (var variant in variants)
                {
                    variant.SetActive(variant.Name == modelName);
                }
                baseModel.SetActive(baseModel.Name == modelName);
            }
        }

        public static ModelBase GetActiveModel(PlayerControllerB player)
        {
            foreach (var baseModel in BaseModels.Values)
            {
                if (baseModel.IsActive && ModelReplacementAPI.GetPlayerModelReplacement(player, out var modelReplacement) && modelReplacement.GetType() == baseModel.Type)
                {
                    return baseModel;
                }
            }

            foreach (var variantList in VariantModels.Values)
            {
                foreach (var variant in variantList)
                {
                    if (variant.IsActive && ModelReplacementAPI.GetPlayerModelReplacement(player, out var modelReplacement) && modelReplacement.GetType() == variant.Type)
                    {
                        return variant;
                    }
                }
            }

            return null;
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
