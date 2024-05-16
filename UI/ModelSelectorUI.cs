using System.Collections.Generic;
using LethalModelSwitcher.Helper;
using LethalModelSwitcher.Utils;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LethalModelSwitcher.UI;

public class ModelSelectorUI : MonoBehaviour
{
    public GameObject mainPanel;
    public Transform previewPanel;
    public RawImage preview;
    public Button previewButton;
    public Transform selectionPanelMain;
    public Transform selectionLayout;
    public Transform optionsPanel;
    public TextMeshProUGUI optionsText;
    public Transform paginationPanel;
    public Button prevButton;
    public Button nextButton;
    public TextMeshProUGUI pageNumberText;
    public Button confirmButton;
    public GameObject lmsCanvasInstance;
    public GameObject buttonPrefab;
    public GameObject modelPreviewPrefab;

    private List<GameObject> modelPreviews = new List<GameObject>();
    private List<Button> dynamicButtons = new List<Button>();
    private List<ModelVariant> models = new List<ModelVariant>();
    private int currentPage = 0;
    private int selectedModelIndex = -1;
    private const int ModelsPerPage = 6;

    private void Awake()
    {
        var lmsCanvasPrefab = HelperTools.LoadUIPrefab("LMSCanvas");
        buttonPrefab = HelperTools.LoadUIPrefab("VariantButtonPrefab");
        modelPreviewPrefab = HelperTools.LoadUIPrefab("ModelPreviewPrefab");
        
        if (lmsCanvasPrefab != null)
        {
            lmsCanvasInstance = Instantiate(lmsCanvasPrefab);
            AssignUIElements(lmsCanvasInstance);
            lmsCanvasInstance.SetActive(false);
        }
        else
        {
            LethalModelSwitcher.Logger.LogError("LMSCanvas prefab not found in the asset bundle.");
        }

        if (prevButton != null) prevButton.onClick.AddListener(PreviousPage);
        if (nextButton != null) nextButton.onClick.AddListener(NextPage);
        if (confirmButton != null) confirmButton.onClick.AddListener(ConfirmSelection);
    }

    public void AssignUIElements(GameObject lmsCanvasInstance)
    {
        mainPanel = lmsCanvasInstance.transform.Find("MainPanel")?.gameObject;
        previewPanel = mainPanel?.transform.Find("PreviewPanel");
        preview = previewPanel?.Find("Preview")?.GetComponent<RawImage>();
        selectionPanelMain = mainPanel?.transform.Find("SelectionPanelMain");
        selectionLayout = selectionPanelMain?.Find("SelectionLayout");
        optionsPanel = mainPanel?.transform.Find("OptionsPanel");
        optionsText = optionsPanel?.Find("Text (TMP)")?.GetComponent<TextMeshProUGUI>();
        paginationPanel = mainPanel?.transform.Find("PaginationPanel");
        prevButton = paginationPanel?.Find("Prev")?.GetComponent<Button>();
        nextButton = paginationPanel?.Find("Next")?.GetComponent<Button>();
        pageNumberText = paginationPanel?.Find("page")?.GetComponent<TextMeshProUGUI>();
        confirmButton = previewPanel?.transform.Find("ConfirmButton")?.GetComponent<Button>();
    }

    public void Open(List<ModelVariant> modelVariants)
    {
        if (lmsCanvasInstance != null)
        {
            lmsCanvasInstance.SetActive(true);
            models = modelVariants;
            currentPage = 0;
            selectedModelIndex = -1;
            UpdatePage();
        }
        else
        {
            LethalModelSwitcher.Logger.LogError("lmsCanvasInstance is null when trying to open the model selector.");
        }
    }

    public void Close()
    {
        if (lmsCanvasInstance != null)
        {
            lmsCanvasInstance.SetActive(false);
        }
        else
        {
            LethalModelSwitcher.Logger.LogError("lmsCanvasInstance is null when trying to close the model selector.");
        }
    }

    private void UpdatePage()
    {
        ClearModelPreviews();
        ClearDynamicButtons();

        int startIndex = currentPage * ModelsPerPage;
        int endIndex = Mathf.Min(startIndex + ModelsPerPage, models.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            int index = i;
            var buttonInstance = Instantiate(buttonPrefab, selectionLayout);
            buttonInstance.GetComponentInChildren<Text>().text = models[i].Name;
            buttonInstance.GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectModel(index);
            });
            dynamicButtons.Add(buttonInstance.GetComponent<Button>());

            var modelPreview = Instantiate(models[i].ModelPrefab, previewPanel);
            modelPreviews.Add(modelPreview);
            modelPreview.transform.localScale = Vector3.one * 0.5f;
            modelPreview.transform.localPosition = new Vector3(0, 0, 0);

            var renderTexture = new RenderTexture(256, 256, 16);
            modelPreview.GetComponentInChildren<Camera>().targetTexture = renderTexture;
            buttonInstance.GetComponentInChildren<RawImage>().texture = renderTexture;
        }

        pageNumberText.text = $"Page {currentPage + 1}/{Mathf.CeilToInt((float)models.Count / ModelsPerPage)}";
    }

    private void ClearModelPreviews()
    {
        foreach (var preview in modelPreviews)
        {
            Destroy(preview);
        }
        modelPreviews.Clear();
    }

    private void ClearDynamicButtons()
    {
        foreach (var button in dynamicButtons)
        {
            Destroy(button.gameObject);
        }
        dynamicButtons.Clear();
    }

    private void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdatePage();
        }
    }

    private void NextPage()
    {
        if ((currentPage + 1) * ModelsPerPage < models.Count)
        {
            currentPage++;
            UpdatePage();
        }
    }

    private void SelectModel(int index)
    {
        var model = models[index];
        preview.texture = model.ModelPrefab.GetComponentInChildren<Camera>().targetTexture;
        selectedModelIndex = index;
    }

    private void ConfirmSelection()
    {
        if (selectedModelIndex >= 0)
        {
            InputHandler.SelectModel(selectedModelIndex);
            Close();
        }
        else
        {
            LethalModelSwitcher.Logger.LogWarning("No model selected to confirm.");
        }
    }
}
