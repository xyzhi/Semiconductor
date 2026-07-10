using UnityEngine;
using UnityEngine.UI;

namespace SemiconductorTeaching
{
    [RequireComponent(typeof(Button))]
    public sealed class PNJunctionAnimationButton : MonoBehaviour
    {
        public int animationIndex = -1;

        Button button;
        PNJunctionTeachingAnimator animator;
        ModelIntroductionPanel introductionPanel;

        void Awake()
        {
            button = GetComponent<Button>();
            animator = FindAnimator();
            introductionPanel = FindObjectOfType<ModelIntroductionPanel>();
            button.onClick.AddListener(PlayAnimation);
        }

        void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(PlayAnimation);
            }
        }

        void PlayAnimation()
        {
            if (animator == null)
            {
                animator = FindAnimator();
            }

            if (animator == null || animationIndex < 0)
            {
                return;
            }

            if (!animator.gameObject.activeInHierarchy)
            {
                return;
            }

            animator.PlayByIndex(animationIndex);
            introductionPanel?.ShowAnimation(animationIndex);
        }

        static PNJunctionTeachingAnimator FindAnimator()
        {
            var animators = FindObjectsOfType<PNJunctionTeachingAnimator>(true);
            return animators.Length > 0 ? animators[0] : null;
        }
    }
}
