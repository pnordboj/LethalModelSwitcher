using System.Linq;
using LethalModelSwitcher.Utils;
using LethalNetworkAPI;
using ModelReplacement;
using UnityEngine;

namespace LethalModelSwitcher.Managers
{
    public class SyncManager
    {
        private static readonly LethalClientMessage<ModelChangeMessage> ToggleModelMessage = new LethalClientMessage<ModelChangeMessage>("LethalModelSwitcher.ToggleModel", null, OnToggleModelReceived);

        public static void SendModelChange(ulong clientId, string suitName, string modelName)
        {
            ToggleModelMessage.SendAllClients(new ModelChangeMessage(clientId, suitName, modelName), false);
        }

        private static void OnToggleModelReceived(ModelChangeMessage message, ulong clientId)
        {
            if (string.IsNullOrEmpty(message.SuitName))
            {
                LethalModelSwitcher.Logger.LogError("OnToggleModelReceived: SuitName is null or empty.");
                return;
            }

            var player = clientId.GetPlayerController();
            if (player != null)
            {
                ModelBase model = null;
                var variants = ModelManager.GetVariants(message.SuitName);

                if (variants != null)
                {
                    model = variants.FirstOrDefault(m => m.Name == message.ModelName);
                }

                if (model == null)
                {
                    // Try getting the base model if no variant found
                    model = ModelManager.GetBaseModel(message.SuitName);
                }

                if (model != null)
                {
                    ModelReplacementAPI.SetPlayerModelReplacement(player, model.Type);

                    if (model.Sound != null)
                    {
                        SoundManager.PlaySound(model.Sound, player.transform.position);
                    }
                }
                else
                {
                    LethalModelSwitcher.Logger.LogError($"Model not found for suit: {message.SuitName} and model: {message.ModelName} in OnToggleModelReceived");
                }
            }
            else
            {
                LethalModelSwitcher.Logger.LogError($"Player not found for client ID: {clientId} in OnToggleModelReceived");
            }
        }
    }

    public class ModelChangeMessage
    {
        public ulong ClientId { get; }
        public string SuitName { get; }
        public string ModelName { get; }

        public ModelChangeMessage(ulong clientId, string suitName, string modelName)
        {
            ClientId = clientId;
            SuitName = suitName;
            ModelName = modelName;
        }
    }
}
