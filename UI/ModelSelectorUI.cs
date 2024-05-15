using System.Collections.Generic;
using LethalModelSwitcher.Utils;
using UnityEngine;
using UnityEngine.UI;

public class ModelSelectorUI : MonoBehaviour
{
    public GameObject panel;
    public Button cycleButton;
    public Transform modelPreviewContainer;
    public GameObject buttonPrefab;
    public Transform contentArea;
    public Button prevButton;
    public Button nextButton;
    public Text pageNumberText;
    public RawImage selectedModelPreview;
    public Button confirmButton;
    public RenderTexture renderTexture;

    private List<GameObject> modelPreviews = new List<GameObject>();
    private List<Button> dynamicButtons = new List<Button>();
    private List<ModelVariant> models = new List<ModelVariant>();
    private int currentPage = 0;
    private const int ModelsPerPage = 6;

    private void Awake()
    {
        panel.SetActive(false);
        cycleButton.onClick.AddListener(() =>
        {
            InputHandler.EnableCycling = true;
            panel.SetActive(false);
        });
        prevButton.onClick.AddListener(PreviousPage);
        nextButton.onClick.AddListener(NextPage);
        confirmButton.onClick.AddListener(ConfirmSelection);
    }

    public void Open(List<ModelVariant> modelVariants)
    {
        panel.SetActive(true);
        InputHandler.EnableCycling = false;
        models = modelVariants;
        currentPage = 0;
        UpdatePage();
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
            var buttonInstance = Instantiate(buttonPrefab, contentArea);
            buttonInstance.GetComponentInChildren<Text>().text = models[i].Name;
            buttonInstance.GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectModel(index);
            });
            dynamicButtons.Add(buttonInstance.GetComponent<Button>());

            var modelPreview = Instantiate(models[i].ModelPrefab, modelPreviewContainer);
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
        selectedModelPreview.texture = model.ModelPrefab.GetComponentInChildren<Camera>().targetTexture;
    }

    private void ConfirmSelection()
    {
        // Logic to confirm the selected model
    }
}
