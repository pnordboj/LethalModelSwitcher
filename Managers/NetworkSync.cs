using System.Linq;
using GameNetcodeStuff;
using LethalModelSwitcher.Utils;
using LethalNetworkAPI;
using ModelReplacement;
using UnityEngine;

namespace LethalModelSwitcher.Managers
{
    public class NetworkSync : MonoBehaviour
    {
        public static NetworkSync Instance { get; private set; }

        private LethalServerMessage<(ulong, string, string)> syncModelServerMessage;
        private LethalClientMessage<(ulong, string, string)> syncModelClientMessage;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            syncModelServerMessage = new LethalServerMessage<(ulong, string, string)>("SyncModelMessage");
            syncModelClientMessage = new LethalClientMessage<(ulong, string, string)>("SyncModelMessage");

            syncModelServerMessage.OnReceived += OnSyncModelMessageReceived;
        }

        public void ChangeModel(ulong clientId, string suitName, string modelName)
        {
            syncModelClientMessage.SendServer((clientId, suitName, modelName));
        }

        private void OnSyncModelMessageReceived((ulong, string, string) data, ulong originatorClientId)
        {
            var (clientId, suitName, modelName) = data;
            SyncModel(clientId, suitName, modelName);
        }

        private void SyncModel(ulong clientId, string suitName, string modelName)
        {
            if (string.IsNullOrEmpty(suitName) || string.IsNullOrEmpty(modelName))
            {
                Debug.LogError("SyncModel: SuitName or ModelName is null or empty.");
                return;
            }

            var player = clientId.GetPlayerController();
            if (player == null)
            {
                Debug.LogError($"Player not found for client ID: {clientId} in SyncModel");
                return;
            }

            ModelBase model = ModelManager.GetVariants(suitName)?.FirstOrDefault(m => m.Name == modelName);
            if (model == null)
            {
                model = ModelManager.GetBaseModel(suitName);
            }

            if (model == null)
            {
                Debug.LogError($"Model not found for suit: {suitName} and model: {modelName} in SyncModel");
                return;
            }

            ModelReplacementAPI.SetPlayerModelReplacement(player, model.Type);

            if (model.Sound != null)
            {
                SoundManager.PlaySound(model.Sound, player.transform.position);
            }
        }
    }
}
