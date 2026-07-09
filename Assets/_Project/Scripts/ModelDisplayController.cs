using System.Collections;
using UnityEngine;

namespace SemiconductorTeaching
{
    public sealed class ModelDisplayController : MonoBehaviour
    {
        public float animationDuration = 0.55f;
        public float enlargedScale = 1.8f;
        public float reducedScale = 0.15f;

        GameObject waferModel;
        GameObject pnJunctionModel;
        Transform pPart;
        Transform nPart;
        ModelIntroductionPanel introductionPanel;
        Vector3 waferNormalScale;
        Vector3 pnNormalScale;
        Vector3 pCombinedPosition;
        Vector3 nCombinedPosition;
        ModelView currentView;
        bool isTransitioning;

        enum ModelView
        {
            Wafer,
            PNJunction,
            PType,
            NType
        }

        void Awake()
        {
            waferModel = GameObject.Find("WaferModel");
            pnJunctionModel = GameObject.Find("PNJunctionModel");

            if (waferModel == null || pnJunctionModel == null)
            {
                Debug.LogError("Scene1 requires WaferModel and PNJunctionModel prefab instances.", this);
                enabled = false;
                return;
            }

            pPart = pnJunctionModel.transform.Find("PPart");
            nPart = pnJunctionModel.transform.Find("NPart");
            introductionPanel = FindObjectOfType<ModelIntroductionPanel>();
            if (pPart == null || nPart == null)
            {
                Debug.LogError("PNJunctionModel requires PPart and NPart child transforms.", this);
                enabled = false;
                return;
            }

            waferNormalScale = waferModel.transform.localScale;
            pnNormalScale = pnJunctionModel.transform.localScale;
            pCombinedPosition = pPart.localPosition;
            nCombinedPosition = nPart.localPosition;
            ShowImmediate(ModelView.Wafer);
            introductionPanel?.Show((int)ModelView.Wafer);
        }

        public void SelectModel(int modelIndex)
        {
            if (!enabled || isTransitioning || modelIndex < 0 || modelIndex > 3)
                return;

            var targetView = (ModelView)modelIndex;
            if (targetView != currentView)
            {
                introductionPanel?.Show(modelIndex);
                StartCoroutine(ChangeModel(targetView));
            }
        }

        IEnumerator ChangeModel(ModelView targetView)
        {
            isTransitioning = true;

            if (currentView == ModelView.Wafer)
            {
                yield return Scale(waferModel.transform, waferNormalScale, waferNormalScale * enlargedScale);
                waferModel.SetActive(false);
                PreparePNJunction(targetView);
                pnJunctionModel.transform.localScale = pnNormalScale * reducedScale;
                pnJunctionModel.SetActive(true);
                yield return Scale(pnJunctionModel.transform, pnNormalScale * reducedScale, pnNormalScale);
            }
            else if (targetView == ModelView.Wafer)
            {
                yield return Scale(pnJunctionModel.transform, pnNormalScale, pnNormalScale * reducedScale);
                pnJunctionModel.SetActive(false);
                waferModel.transform.localScale = waferNormalScale * enlargedScale;
                waferModel.SetActive(true);
                yield return Scale(waferModel.transform, waferNormalScale * enlargedScale, waferNormalScale);
            }
            else
            {
                yield return ChangePNJunctionView(targetView);
            }

            currentView = targetView;
            isTransitioning = false;
        }

        IEnumerator ChangePNJunctionView(ModelView targetView)
        {
            pPart.gameObject.SetActive(true);
            nPart.gameObject.SetActive(true);

            var pTarget = targetView == ModelView.PType ? Vector3.zero : pCombinedPosition;
            var nTarget = targetView == ModelView.NType ? Vector3.zero : nCombinedPosition;
            var elapsed = 0f;
            var pStart = pPart.localPosition;
            var nStart = nPart.localPosition;

            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                var t = Smooth(elapsed / animationDuration);
                pPart.localPosition = Vector3.LerpUnclamped(pStart, pTarget, t);
                nPart.localPosition = Vector3.LerpUnclamped(nStart, nTarget, t);
                yield return null;
            }

            pPart.localPosition = pTarget;
            nPart.localPosition = nTarget;
            pPart.gameObject.SetActive(targetView != ModelView.NType);
            nPart.gameObject.SetActive(targetView != ModelView.PType);
        }

        void PreparePNJunction(ModelView targetView)
        {
            pPart.localPosition = targetView == ModelView.PType ? Vector3.zero : pCombinedPosition;
            nPart.localPosition = targetView == ModelView.NType ? Vector3.zero : nCombinedPosition;
            pPart.gameObject.SetActive(targetView != ModelView.NType);
            nPart.gameObject.SetActive(targetView != ModelView.PType);
        }

        void ShowImmediate(ModelView targetView)
        {
            waferModel.transform.localScale = waferNormalScale;
            pnJunctionModel.transform.localScale = pnNormalScale;
            waferModel.SetActive(targetView == ModelView.Wafer);
            pnJunctionModel.SetActive(targetView != ModelView.Wafer);
            PreparePNJunction(targetView);
            currentView = targetView;
        }

        IEnumerator Scale(Transform target, Vector3 from, Vector3 to)
        {
            var elapsed = 0f;
            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                target.localScale = Vector3.LerpUnclamped(from, to, Smooth(elapsed / animationDuration));
                yield return null;
            }

            target.localScale = to;
        }

        static float Smooth(float value)
        {
            value = Mathf.Clamp01(value);
            return value * value * (3f - 2f * value);
        }
    }
}
