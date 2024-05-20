using UnityEngine;

namespace LethalModelSwitcher.UI
{
    public class ModelPreviewer : MonoBehaviour
    {
        public GameObject modelPrefab;
        private GameObject modelInstance;
        private SkinnedMeshRenderer skinnedMeshRenderer;

        public void SetupPreview(GameObject prefab = null)
        {
            if (prefab != null)
            {
                modelPrefab = prefab;
            }

            if (modelPrefab == null)
            {
                Debug.LogError("ModelPrefab is null. Cannot setup preview.");
                return;
            }

            modelInstance = Instantiate(modelPrefab, transform);
            skinnedMeshRenderer = modelInstance.GetComponentInChildren<SkinnedMeshRenderer>();

            if (skinnedMeshRenderer == null)
            {
                Debug.LogError("SkinnedMeshRenderer is null. Cannot setup preview.");
                return;
            }

            skinnedMeshRenderer.material = new Material(skinnedMeshRenderer.material);
        }
        
        public void SetMaterial(Material material)
        {
            if (skinnedMeshRenderer == null)
            {
                Debug.LogError("SkinnedMeshRenderer is null. Cannot set material.");
                return;
            }

            skinnedMeshRenderer.material = material;
        }
    }
}