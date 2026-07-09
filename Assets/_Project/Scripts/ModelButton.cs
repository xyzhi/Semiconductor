using UnityEngine;
using UnityEngine.UI;

namespace SemiconductorTeaching
{
    [RequireComponent(typeof(Button))]
    public sealed class ModelButton : MonoBehaviour
    {
        public int modelIndex;

        Button button;
        ModelDisplayController controller;

        void Awake()
        {
            button = GetComponent<Button>();
            controller = GetComponentInParent<ModelDisplayController>();
            button.onClick.AddListener(SelectModel);
        }

        void OnDestroy()
        {
            if (button != null)
                button.onClick.RemoveListener(SelectModel);
        }

        void SelectModel()
        {
            if (controller != null)
                controller.SelectModel(modelIndex);
        }
    }
}
