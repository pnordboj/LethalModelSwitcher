using System;
using System.Linq;
using LethalModelSwitcher.Utils;
using LethalNetworkAPI;
using ModelReplacement;
using Unity.Netcode;
using UnityEngine;

namespace LethalModelSwitcher.Managers
{
    public class SyncManager
    {
        private static readonly LethalServerMessage<ModelChangeMessage> ServerModelChangeMessage = new LethalServerMessage<ModelChangeMessage>("LethalModelSwitcher.ServerModelChange");
        private static readonly LethalClientMessage<ModelChangeMessage> ClientModelChangeMessage = new LethalClientMessage<ModelChangeMessage>("LethalModelSwitcher.ClientModelChange");

        static SyncManager()
        {
            ServerModelChangeMessage.OnReceived += OnServerModelChangeReceived;
            ClientModelChangeMessage.OnReceived += OnClientModelChangeReceived;
        }

        public static void SendModelChange(ulong clientId, string suitName, string modelName)
        {
            if (string.IsNullOrEmpty(suitName) || string.IsNullOrEmpty(modelName))
            {
                plugin.Logger.LogError("SendModelChange: SuitName or ModelName is null or empty.");
                return;
            }

            ServerModelChangeMessage.SendAllClients(new ModelChangeMessage(clientId, suitName, modelName), true);
        }

        private static void OnServerModelChangeReceived(ModelChangeMessage message, ulong clientId)
        {
            if (IsServer())
            {
                ServerModelChangeMessage.SendAllClients(message);
            }
        }

        private static void OnClientModelChangeReceived(ModelChangeMessage message)
        {
            var player = message.ClientId.GetPlayerController();
            if (player == null)
            {
                plugin.Logger.LogError($"Player not found for client ID: {message.ClientId} in OnClientModelChangeReceived");
                return;
            }

            ModelBase model = ModelManager.GetVariants(message.SuitName)?.FirstOrDefault(m => m.Name == message.ModelName) as ModelBase
                              ?? ModelManager.GetBaseModel(message.SuitName);

            if (model == null)
            {
                plugin.Logger.LogError($"Model not found for suit: {message.SuitName} and model: {message.ModelName} in OnClientModelChangeReceived");
                return;
            }

            ModelReplacementAPI.SetPlayerModelReplacement(player, model.Type);

            if (model.Sound != null)
            {
                SoundManager.PlaySound(model.Sound, player.transform.position);
            }
        }
        
        private static bool IsServer()
        {
            // Implement your logic to check if the current instance is the server.
            return NetworkManager.Singleton.IsServer;
        }
    }

    public class ModelChangeMessage
    {
        public ulong ClientId { get; }
        public string SuitName { get; }
        public string ModelName { get; }

        public ModelChangeMessage(ulong clientId, string suitName, string modelName)
        {
            if (string.IsNullOrEmpty(suitName))
            {
                throw new ArgumentNullException(nameof(suitName), "SuitName cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(modelName))
            {
                throw new ArgumentNullException(nameof(modelName), "ModelName cannot be null or empty.");
            }

            ClientId = clientId;
            SuitName = suitName;
            ModelName = modelName;
        }
    }
}
