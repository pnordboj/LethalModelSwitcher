using HarmonyLib;
using UnityEngine.InputSystem;
using ModelReplacement;
using LethalNetworkAPI;
using UnityEngine;
using System.Linq;
using GameNetcodeStuff;
using LethalModelSwitcher.Utils;

[HarmonyPatch(typeof(PlayerControllerB))]
public class InputHandler : MonoBehaviour
{
    public static PlayerControllerB LocalPlayerController;
    public static bool EnableCycling = true;
    public static ModelSelectorUI ModelSelectorUI;

    private static readonly LethalClientMessage<ModelChangeMessage> ToggleModelMessage = new LethalClientMessage<ModelChangeMessage>("LethalModelSwitcher.ToggleModel", null, OnToggleModelReceived);

    [HarmonyPatch("Awake")]
    [HarmonyPrefix]
    private static void Awake(PlayerControllerB __instance)
    {
        if (__instance.IsOwner)
        {
            LocalPlayerController = __instance;
        }
        InputActions.Instance.ToggleModelKey.performed += OnToggleModelKeyPressed;
        InputActions.Instance.OpenModelSelectorKey.performed += OnOpenModelSelectorKeyPressed;

        // Assuming ModelSelectorUI is a prefab you instantiated and assigned
        ModelSelectorUI = FindObjectOfType<ModelSelectorUI>();
    }

    private static void OnToggleModelKeyPressed(InputAction.CallbackContext context)
    {
        if (context.performed && EnableCycling)
        {
            ToggleModel();
        }
    }

    private static void OnOpenModelSelectorKeyPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var currentModel = LocalPlayerController.GetComponent<BodyReplacementBase>().suitName;
            if (ModelManager.RegisteredModels.ContainsKey(currentModel))
            {
                ModelSelectorUI.Open(ModelManager.RegisteredModels[currentModel]);
            }
        }
    }

    public static void ToggleModel()
    {
        var currentModel = LocalPlayerController.GetComponent<BodyReplacementBase>().suitName;
        if (!ModelManager.RegisteredModels.ContainsKey(currentModel)) return;

        var models = ModelManager.RegisteredModels[currentModel];
        var currentIndex = models.FindIndex(m => m.IsActive);
        var nextIndex = (currentIndex + 1) % models.Count;

        SelectModel(nextIndex);
    }

    public static void SelectModel(int index)
    {
        var currentModel = LocalPlayerController.GetComponent<BodyReplacementBase>().suitName;
        var models = ModelManager.RegisteredModels[currentModel];

        var nextModel = models[index];
        ModelReplacementAPI.SetPlayerModelReplacement(LocalPlayerController, nextModel.Type);
        nextModel.SetActive(true);
        models.ForEach(m => m.SetActive(false));
        nextModel.SetActive(true);

        if (nextModel.Sound != null)
        {
            LethalModelSwitcher.Utils.SoundManager.PlaySound(nextModel.Sound, LocalPlayerController.transform.position);
        }

        ToggleModelMessage.SendAllClients(new ModelChangeMessage(LocalPlayerController.GetClientId(), nextModel.Name), false);
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
                LethalModelSwitcher.Utils.SoundManager.PlaySound(model.Sound, player.transform.position);
            }
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
