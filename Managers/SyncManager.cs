using System.Linq;
using LethalNetworkAPI;
using ModelReplacement;
using UnityEngine;

namespace LethalModelSwitcher.Utils
{
    public class SyncManager
    {
        private static readonly LethalClientMessage<ModelChangeMessage> ToggleModelMessage = new LethalClientMessage<ModelChangeMessage>("LethalModelSwitcher.ToggleModel", null, OnToggleModelReceived);

        public static void SendModelChange(ulong clientId, string modelName)
        {
            ToggleModelMessage.SendAllClients(new ModelChangeMessage(clientId, modelName), false);
        }

        private static void OnToggleModelReceived(ModelChangeMessage message, ulong clientId)
        {
            var player = clientId.GetPlayerController();
            if (player != null)
            {
                var models = ModelManager.RegisteredModels[message.ModelName];
                var model = models.First(m => m.Name == message.ModelName);
                ModelReplacementAPI.SetPlayerModelReplacement(player, model.Type);

                if (model.Sound != null)
                {
                    SoundManager.PlaySound(model.Sound, player.transform.position);
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
        public string ModelName { get; }

        public ModelChangeMessage(ulong clientId, string modelName)
        {
            ClientId = clientId;
            ModelName = modelName;
        }
    }
}