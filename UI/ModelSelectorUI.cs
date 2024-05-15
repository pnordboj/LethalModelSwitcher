using System.Collections.Generic;
using LethalModelSwitcher.Utils;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        var lmsCanvasPrefab = AssetLoader.LoadUIPrefab("LMSCanvas");
        buttonPrefab = AssetLoader.LoadUIPrefab("VariantButtonPrefab");
        modelPreviewPrefab = AssetLoader.LoadUIPrefab("ModelPreviewPrefab");
        

        if (lmsCanvasPrefab != null)
        {
            lmsCanvasInstance = Instantiate(lmsCanvasPrefab);
            AssignUIElements(lmsCanvasInstance);
            lmsCanvasInstance.SetActive(false);
        }
        else
        {
            Debug.LogError("LMSCanvas prefab not found in the asset bundle.");
        }

        if (prevButton != null) prevButton.onClick.AddListener(PreviousPage);
        if (nextButton != null) nextButton.onClick.AddListener(NextPage);
        if (confirmButton != null) confirmButton.onClick.AddListener(ConfirmSelection);
    }

    private void AssignUIElements(GameObject lmsCanvasInstance)
    {
        if (lmsCanvasInstance == null)
        {
            Debug.LogError("lmsCanvasInstance is null.");
            return;
        }

        mainPanel = lmsCanvasInstance.transform.Find("MainPanel")?.gameObject;
        if (mainPanel == null) Debug.LogError("mainPanel not found.");

        previewPanel = mainPanel?.transform.Find("PreviewPanel");
        if (previewPanel == null) Debug.LogError("previewPanel not found.");

        preview = previewPanel?.Find("Preview")?.GetComponent<RawImage>();
        if (preview == null) Debug.LogError("preview not found.");

        selectionPanelMain = mainPanel?.transform.Find("SelectionPanelMain");
        if (selectionPanelMain == null) Debug.LogError("selectionPanelMain not found.");

        selectionLayout = selectionPanelMain?.Find("SelectionLayout");
        if (selectionLayout == null) Debug.LogError("selectionLayout not found.");

        optionsPanel = mainPanel?.transform.Find("OptionsPanel");
        if (optionsPanel == null) Debug.LogError("optionsPanel not found.");

        optionsText = optionsPanel?.Find("Text (TMP)")?.GetComponent<TextMeshProUGUI>();
        if (optionsText == null) Debug.LogError("optionsText not found.");

        paginationPanel = mainPanel?.transform.Find("PaginationPanel");
        if (paginationPanel == null) Debug.LogError("paginationPanel not found.");

        prevButton = paginationPanel?.Find("Prev")?.GetComponent<Button>();
        if (prevButton == null) Debug.LogError("prevButton not found.");

        nextButton = paginationPanel?.Find("Next")?.GetComponent<Button>();
        if (nextButton == null) Debug.LogError("nextButton not found.");

        pageNumberText = paginationPanel?.Find("page")?.GetComponent<TextMeshProUGUI>();
        if (pageNumberText == null) Debug.LogError("pageNumberText not found.");

        confirmButton = previewPanel?.transform.Find("ConfirmButton")?.GetComponent<Button>();
        if (confirmButton == null) Debug.LogError("confirmButton not found.");
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
            Debug.LogError("lmsCanvasInstance is null when trying to open the model selector.");
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
            Debug.LogError("lmsCanvasInstance is null when trying to close the model selector.");
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
            modelPreview.transform.localScale = Vector3.one * 0.5f; // Adjust the scale if needed
            modelPreview.transform.localPosition = new Vector3(0, 0, 0); // Adjust the position if needed

            // Set up render texture for the model preview
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
            Debug.LogWarning("No model selected to confirm.");
        }
    }
}
