using System.Linq;
using LethalModelSwitcher.Utils;
using LethalNetworkAPI;
using ModelReplacement;

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
            var player = clientId.GetPlayerController();
            if (player != null)
            {
                var models = ModelManager.GetVariants(message.SuitName);
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