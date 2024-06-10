using System.Collections.Generic;
using HarmonyLib;
using LethalModelSwitcher.Helper;
using LethalModelSwitcher.Managers;
using LethalModelSwitcher.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LethalModelSwitcher.UI
{
    [HarmonyPatch]
    public class ModelSelectorUI : MonoBehaviour
    {
        public static ModelSelectorUI Instance { get; private set; }

        public static GameObject mainPanel;
        public static Transform previewPanel;
        public static SkinnedMeshRenderer modelRenderer;
        public static Camera previewCamera;
        public static Camera buttonPreviewCamera;
        public Button previewButton;
        public static Transform selectionPanelMain;
        public static Transform selectionLayout;
        public static Transform optionsPanel;
        public static TextMeshProUGUI optionsText;
        public static Transform paginationPanel;
        public static Button prevButton;
        public static Button nextButton;
        public static TextMeshProUGUI pageNumberText;
        public static Button confirmButton;
        public static GameObject lmsCanvasInstance;
        public static GameObject buttonPrefab;

        private static List<GameObject> modelPreviews = new List<GameObject>();
        private static List<Button> dynamicButtons = new List<Button>();
        private static List<ModelVariant> models = new List<ModelVariant>();
        private static int currentPage = 0;
        private static int selectedModelIndex = -1;
        private const int ModelsPerPage = 6;

        [HarmonyPatch(typeof(HUDManager), "Awake")]
        [HarmonyPostfix]
        public static void InitUI(HUDManager __instance)
        {
            CustomLogging.Log("ModelSelectorUI: InitUI called.");
            
            if (!plugin.Instance.loadUI)
            {
                CustomLogging.Log("ModelSelectorUI: UI loading is disabled.");
                return;
            }

            if (Instance == null)
            {
                CustomLogging.Log("ModelSelectorUI: Instance is null, initializing...");

                var modelSelectorPrefab = HelperTools.LoadUIPrefab("LMSCanvas");
                if (modelSelectorPrefab != null)
                {
                    var modelSelectorInstance = Instantiate(modelSelectorPrefab);
                    Instance = modelSelectorInstance.AddComponent<ModelSelectorUI>();
                    ModelSelectorUI.AssignUIElements(modelSelectorInstance);
                    ModelSelectorUI.lmsCanvasInstance = modelSelectorInstance;
                    modelSelectorInstance.SetActive(false);
                    ModelSelectorUI.buttonPrefab = HelperTools.LoadUIPrefab("ModelButton");

                    CustomLogging.Log("ModelSelectorUI: Initialization complete.");
                }
                else
                {
                    CustomLogging.LogError("ModelSelectorUI: Failed to load LMSCanvas prefab.");
                }
            }
            else
            {
                CustomLogging.Log("ModelSelectorUI: Instance already exists.");
            }
        }

        public static void AssignUIElements(GameObject lmsCanvasInstance)
        {
            CustomLogging.Log("Assigning UI elements for ModelSelectorUI.");

            mainPanel = lmsCanvasInstance.transform.Find("LMSPanel")?.gameObject;
            RectTransform mainPanelRectTransform = mainPanel.GetComponent<RectTransform>();
            mainPanelRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            mainPanelRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            mainPanelRectTransform.pivot = new Vector2(0.5f, 0.5f);
            mainPanelRectTransform.sizeDelta = new Vector2(800, 600); // Adjust the size as needed
            mainPanelRectTransform.anchoredPosition = Vector2.zero;

            previewPanel = mainPanel?.transform.Find("PreviewPanel");
            selectionPanelMain = mainPanel?.transform.Find("SelectionPanelMain");
            selectionLayout = selectionPanelMain?.Find("SelectionLayout");
            //optionsPanel = mainPanel?.transform.Find("OptionsPanel");
            //optionsText = optionsPanel?.Find("OptionsText")?.GetComponent<TextMeshProUGUI>();
            paginationPanel = mainPanel?.transform.Find("PaginationPanel");
            prevButton = paginationPanel?.Find("Prev")?.GetComponent<Button>();
            nextButton = paginationPanel?.Find("Next")?.GetComponent<Button>();
            pageNumberText = paginationPanel?.Find("PageNumber")?.GetComponent<TextMeshProUGUI>();
            confirmButton = previewPanel?.Find("ConfirmButton")?.GetComponent<Button>();

            if (prevButton != null) prevButton.onClick.AddListener(PreviousPage);
            if (nextButton != null) nextButton.onClick.AddListener(NextPage);
            if (confirmButton != null) confirmButton.onClick.AddListener(ConfirmSelection);

            CustomLogging.Log("ModelSelectorUI: UI elements assigned.");
        }

        public void Open(List<ModelVariant> modelVariants)
        {
            if (lmsCanvasInstance != null)
            {
                CustomLogging.Log("ModelSelectorUI: Opening model selector.");

                lmsCanvasInstance.SetActive(true);
                models = modelVariants;
                currentPage = 0;
                selectedModelIndex = -1;
                UpdatePage();

                // Unlock and show the cursor
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                CustomLogging.LogError("ModelSelectorUI: lmsCanvasInstance is null when trying to open the model selector.");
            }
        }

        public static void Close()
        {
            if (lmsCanvasInstance != null)
            {
                CustomLogging.Log("ModelSelectorUI: Closing model selector.");

                lmsCanvasInstance.SetActive(false);

                // Lock and hide the cursor
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                CustomLogging.LogError("ModelSelectorUI: lmsCanvasInstance is null when trying to close the model selector.");
            }
        }

        private static void SetupPreviewCamera()
        {
            previewCamera = new GameObject("PreviewCamera").AddComponent<Camera>();
            GameObject.Destroy(previewCamera.GetComponent<AudioListener>());
            previewCamera.transform.SetParent(previewPanel);
            previewCamera.cullingMask = LayerMask.GetMask("UI");
            previewCamera.clearFlags = CameraClearFlags.SolidColor;
            previewCamera.cameraType = CameraType.Preview;
            previewCamera.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0);
            previewCamera.allowHDR = false;
            previewCamera.allowMSAA = false;
            previewCamera.farClipPlane = 5;
            previewCamera.transform.localPosition = Vector3.zero;

            Light spotlight = new GameObject("Spotlight").AddComponent<Light>();
            spotlight.type = LightType.Spot;
            spotlight.transform.SetParent(previewCamera.transform);
            spotlight.transform.localPosition = Vector3.zero;
            spotlight.transform.localRotation = Quaternion.identity;
            spotlight.intensity = 50;
            spotlight.range = 40;
            spotlight.innerSpotAngle = 100;
            spotlight.spotAngle = 120;
            spotlight.gameObject.layer = 23;
        }

        private static void SetupButtonPreviewCamera()
        {
            buttonPreviewCamera = new GameObject("ButtonPreviewCamera").AddComponent<Camera>();
            GameObject.Destroy(buttonPreviewCamera.GetComponent<AudioListener>());
            buttonPreviewCamera.transform.SetParent(selectionLayout);
            buttonPreviewCamera.cullingMask = LayerMask.GetMask("UI");
            buttonPreviewCamera.clearFlags = CameraClearFlags.SolidColor;
            buttonPreviewCamera.cameraType = CameraType.Preview;
            buttonPreviewCamera.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0);
            buttonPreviewCamera.allowHDR = false;
            buttonPreviewCamera.allowMSAA = false;
            buttonPreviewCamera.farClipPlane = 5;
            buttonPreviewCamera.transform.localPosition = Vector3.zero;

            Light spotlight = new GameObject("Spotlight").AddComponent<Light>();
            spotlight.type = LightType.Spot;
            spotlight.transform.SetParent(buttonPreviewCamera.transform);
            spotlight.transform.localPosition = Vector3.zero;
            spotlight.transform.localRotation = Quaternion.identity;
            spotlight.intensity = 50;
            spotlight.range = 40;
            spotlight.innerSpotAngle = 100;
            spotlight.spotAngle = 120;
            spotlight.gameObject.layer = 23;
        }

        private static void UpdatePage()
        {
            ClearModelPreviews();
            ClearDynamicButtons();

            var startIndex = currentPage * ModelsPerPage;
            var endIndex = Mathf.Min((currentPage + 1) * ModelsPerPage, models.Count);
            for (var i = startIndex; i < endIndex; i++)
            {
                var modelIndex = i; // Capture the correct index for the button click
                var model = models[i];
                var button = Instantiate(buttonPrefab, selectionLayout).GetComponent<Button>();
                button.onClick.AddListener(() => SelectModel(modelIndex));
                button.GetComponentInChildren<TextMeshProUGUI>().text = model.Name; // Use variant name instead of suit name
                dynamicButtons.Add(button);
            }

            pageNumberText.text = $"{currentPage + 1}/{Mathf.CeilToInt((float)models.Count / ModelsPerPage)}";
        }

        private static void ClearModelPreviews()
        {
            CustomLogging.Log("ModelSelectorUI: Clearing model previews.");
            foreach (var preview in modelPreviews)
            {
                Destroy(preview);
            }
            modelPreviews.Clear();
        }

        private static void ClearDynamicButtons()
        {
            CustomLogging.Log("ModelSelectorUI: Clearing dynamic buttons.");
            foreach (var button in dynamicButtons)
            {
                Destroy(button.gameObject);
            }
            dynamicButtons.Clear();
        }

        private static void PreviousPage()
        {
            if (currentPage > 0)
            {
                currentPage--;
                UpdatePage();
            }
        }

        private static void NextPage()
        {
            if ((currentPage + 1) * ModelsPerPage < models.Count)
            {
                currentPage++;
                UpdatePage();
            }
        }

        private static void SelectModel(int index)
        {
            if (index >= 0 && index < models.Count)
            {
                selectedModelIndex = index;
                CustomLogging.Log($"ModelSelectorUI: Selected model index: {index}");
                optionsText.text = $"Selected model: {models[index].Name}";
                DisplayModelPreview(models[index].ModelPrefab);
            }
            else
            {
                CustomLogging.LogError("SelectModel: Index out of range.");
            }
        }

        private static void DisplayModelPreview(GameObject modelPrefab)
        {
            ClearModelPreviews();
            var preview = Instantiate(modelPrefab, previewPanel);
            modelPreviews.Add(preview);
            preview.transform.localPosition = Vector3.zero;
            preview.transform.localRotation = Quaternion.identity;
            preview.transform.localScale = Vector3.one;
            preview.transform.localPosition = new Vector3(0, -0.5f, 0);
            preview.transform.localRotation = Quaternion.Euler(0, 180, 0);
            preview.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }

        private static void ConfirmSelection()
        {
            if (selectedModelIndex >= 0)
            {
                InputHandler.SelectModel(selectedModelIndex, models[selectedModelIndex].SuitName);
                Close();
            }
            else
            {
                plugin.Logger.LogWarning("No model selected to confirm.");
            }
        }
    }
}
