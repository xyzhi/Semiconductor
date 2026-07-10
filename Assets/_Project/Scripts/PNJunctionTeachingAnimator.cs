using System.Collections;
using UnityEngine;

namespace SemiconductorTeaching
{
    public sealed class PNJunctionTeachingAnimator : MonoBehaviour
    {
        public bool playOnEnable;
        public float cycleDuration = 3.4f;
        public float pauseDuration = 0.45f;
        public float diffusionPortion = 0.48f;
        public float recombinationPortion = 0.24f;
        public float carrierMoveDistance = 0.075f;
        public float recombinedCarrierScale = 0.22f;
        public float ionPulseScale = 1.55f;
        public float depletionPulseScale = 1.35f;

        Transform[] electrons;
        Transform[] holes;
        Transform[] fixedIons;
        Transform depletionLayer;
        Vector3[] electronStartPositions;
        Vector3[] holeStartPositions;
        Vector3[] electronStartScales;
        Vector3[] holeStartScales;
        Vector3[] ionStartScales;
        Vector3 depletionLayerStartScale;
        Coroutine animationRoutine;

        enum AnimationSegment
        {
            Diffusion,
            Recombination,
            Depletion,
            Full,
            Reset
        }

        void Awake()
        {
            electrons = FindChildren("NPart/Electrons");
            holes = FindChildren("PPart/Holes");
            fixedIons = Combine(
                FindChildren("MiddlePart/FixedNegativeIons"),
                FindChildren("MiddlePart/FixedPositiveIons"));
            depletionLayer = transform.Find("MiddlePart/Cube");

            electronStartPositions = ReadLocalPositions(electrons);
            holeStartPositions = ReadLocalPositions(holes);
            electronStartScales = ReadLocalScales(electrons);
            holeStartScales = ReadLocalScales(holes);
            ionStartScales = ReadLocalScales(fixedIons);
            depletionLayerStartScale = depletionLayer != null ? depletionLayer.localScale : Vector3.one;
        }

        void OnEnable()
        {
            if (playOnEnable)
            {
                Play();
            }
        }

        void OnDisable()
        {
            Stop();
        }

        public void Play()
        {
            PlayFull();
        }

        public void PlayByIndex(int animationIndex)
        {
            if (animationIndex < 0 || animationIndex > 4)
            {
                return;
            }

            var segment = (AnimationSegment)animationIndex;
            switch (segment)
            {
                case AnimationSegment.Diffusion:
                    PlayDiffusion();
                    break;
                case AnimationSegment.Recombination:
                    PlayRecombination();
                    break;
                case AnimationSegment.Depletion:
                    PlayDepletion();
                    break;
                case AnimationSegment.Full:
                    PlayFull();
                    break;
                case AnimationSegment.Reset:
                    Stop();
                    break;
            }
        }

        public void PlayDiffusion()
        {
            StartAnimation(AnimateSegment(0f, Mathf.Clamp01(diffusionPortion), cycleDuration * Mathf.Clamp01(diffusionPortion)));
        }

        public void PlayRecombination()
        {
            var diffusionEnd = Mathf.Clamp01(diffusionPortion);
            var recombinationEnd = Mathf.Clamp01(diffusionEnd + recombinationPortion);
            StartAnimation(AnimateSegment(diffusionEnd, recombinationEnd, cycleDuration * Mathf.Max(0.01f, recombinationEnd - diffusionEnd)));
        }

        public void PlayDepletion()
        {
            var diffusionEnd = Mathf.Clamp01(diffusionPortion);
            var recombinationEnd = Mathf.Clamp01(diffusionEnd + recombinationPortion);
            StartAnimation(AnimateSegment(recombinationEnd, 1f, cycleDuration * Mathf.Max(0.01f, 1f - recombinationEnd)));
        }

        public void PlayFull()
        {
            StartAnimation(AnimateFull());
        }

        void StartAnimation(IEnumerator routine)
        {
            if (animationRoutine != null)
            {
                StopCoroutine(animationRoutine);
            }

            animationRoutine = StartCoroutine(routine);
        }

        public void Stop()
        {
            if (animationRoutine != null)
            {
                StopCoroutine(animationRoutine);
                animationRoutine = null;
            }

            ResetState();
        }

        IEnumerator AnimateFull()
        {
            yield return AnimateRange(0f, 1f, cycleDuration);
            if (pauseDuration > 0f)
            {
                yield return new WaitForSeconds(pauseDuration);
            }

            yield return RestoreOverTime();
            animationRoutine = null;
        }

        IEnumerator AnimateSegment(float from, float to, float duration)
        {
            yield return AnimateRange(from, to, duration);
            animationRoutine = null;
        }

        IEnumerator AnimateRange(float from, float to, float duration)
        {
            var elapsed = 0f;
            duration = Mathf.Max(0.01f, duration);
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = Smooth(elapsed / duration);
                ApplyCycle(Mathf.Lerp(from, to, t));
                yield return null;
            }

            ApplyCycle(to);
        }

        IEnumerator RestoreOverTime()
        {
            var resetElapsed = 0f;
            while (resetElapsed < pauseDuration)
            {
                resetElapsed += Time.deltaTime;
                var t = Smooth(resetElapsed / Mathf.Max(0.01f, pauseDuration));
                RestoreCarriers(electrons, electronStartPositions, electronStartScales, t);
                RestoreCarriers(holes, holeStartPositions, holeStartScales, t);
                PulseIons(1f - t);
                PulseDepletionLayer(1f - t);
                yield return null;
            }

            ResetState();
        }

        void ApplyCycle(float cycleTime)
        {
            var diffusionEnd = Mathf.Clamp01(diffusionPortion);
            var recombinationEnd = Mathf.Clamp01(diffusionEnd + recombinationPortion);

            if (cycleTime <= diffusionEnd)
            {
                var t = Smooth(cycleTime / Mathf.Max(0.01f, diffusionEnd));
                MoveCarriers(electrons, electronStartPositions, electronStartScales, Vector3.left, t, 1f);
                MoveCarriers(holes, holeStartPositions, holeStartScales, Vector3.right, t, 1f);
                PulseIons(t * 0.35f);
                PulseDepletionLayer(t * 0.35f);
                return;
            }

            if (cycleTime <= recombinationEnd)
            {
                var t = Smooth((cycleTime - diffusionEnd) / Mathf.Max(0.01f, recombinationEnd - diffusionEnd));
                MoveCarriers(electrons, electronStartPositions, electronStartScales, Vector3.left, 1f, Mathf.Lerp(1f, recombinedCarrierScale, t));
                MoveCarriers(holes, holeStartPositions, holeStartScales, Vector3.right, 1f, Mathf.Lerp(1f, recombinedCarrierScale, t));
                PulseIons(Mathf.Lerp(0.35f, 1f, t));
                PulseDepletionLayer(Mathf.Lerp(0.35f, 1f, t));
                return;
            }

            var pulseT = (cycleTime - recombinationEnd) / Mathf.Max(0.01f, 1f - recombinationEnd);
            var pulse = 0.75f + 0.25f * Mathf.Sin(pulseT * Mathf.PI * 4f);
            MoveCarriers(electrons, electronStartPositions, electronStartScales, Vector3.left, 1f, recombinedCarrierScale);
            MoveCarriers(holes, holeStartPositions, holeStartScales, Vector3.right, 1f, recombinedCarrierScale);
            PulseIons(pulse);
            PulseDepletionLayer(pulse);
        }

        void MoveCarriers(
            Transform[] carriers,
            Vector3[] startPositions,
            Vector3[] startScales,
            Vector3 direction,
            float moveT,
            float scaleMultiplier)
        {
            for (var i = 0; i < carriers.Length; i++)
            {
                if (carriers[i] != null)
                {
                    carriers[i].localPosition = startPositions[i] + direction * carrierMoveDistance * moveT;
                    carriers[i].localScale = startScales[i] * scaleMultiplier;
                }
            }
        }

        void PulseIons(float wave)
        {
            for (var i = 0; i < fixedIons.Length; i++)
            {
                if (fixedIons[i] != null)
                {
                    fixedIons[i].localScale = ionStartScales[i] * Mathf.Lerp(1f, ionPulseScale, wave);
                }
            }
        }

        void PulseDepletionLayer(float pulse)
        {
            if (depletionLayer == null)
            {
                return;
            }

            var scale = depletionLayerStartScale;
            scale.x *= Mathf.Lerp(1f, depletionPulseScale, pulse);
            depletionLayer.localScale = scale;
        }

        void RestoreCarriers(Transform[] carriers, Vector3[] startPositions, Vector3[] startScales, float t)
        {
            for (var i = 0; i < carriers.Length; i++)
            {
                if (carriers[i] != null)
                {
                    carriers[i].localPosition = Vector3.Lerp(carriers[i].localPosition, startPositions[i], t);
                    carriers[i].localScale = Vector3.Lerp(carriers[i].localScale, startScales[i], t);
                }
            }
        }

        void ResetState()
        {
            RestoreLocalPositions(electrons, electronStartPositions);
            RestoreLocalPositions(holes, holeStartPositions);
            RestoreLocalScales(electrons, electronStartScales);
            RestoreLocalScales(holes, holeStartScales);
            RestoreLocalScales(fixedIons, ionStartScales);
            if (depletionLayer != null)
            {
                depletionLayer.localScale = depletionLayerStartScale;
            }
        }

        Transform[] FindChildren(string path)
        {
            var parent = transform.Find(path);
            if (parent == null)
            {
                return new Transform[0];
            }

            var children = new Transform[parent.childCount];
            for (var i = 0; i < parent.childCount; i++)
            {
                children[i] = parent.GetChild(i);
            }

            return children;
        }

        static Transform[] Combine(Transform[] first, Transform[] second)
        {
            var combined = new Transform[first.Length + second.Length];
            first.CopyTo(combined, 0);
            second.CopyTo(combined, first.Length);
            return combined;
        }

        static Vector3[] ReadLocalPositions(Transform[] transforms)
        {
            var positions = new Vector3[transforms.Length];
            for (var i = 0; i < transforms.Length; i++)
            {
                positions[i] = transforms[i] != null ? transforms[i].localPosition : Vector3.zero;
            }

            return positions;
        }

        static Vector3[] ReadLocalScales(Transform[] transforms)
        {
            var scales = new Vector3[transforms.Length];
            for (var i = 0; i < transforms.Length; i++)
            {
                scales[i] = transforms[i] != null ? transforms[i].localScale : Vector3.one;
            }

            return scales;
        }

        static void RestoreLocalPositions(Transform[] transforms, Vector3[] positions)
        {
            for (var i = 0; i < transforms.Length; i++)
            {
                if (transforms[i] != null)
                {
                    transforms[i].localPosition = positions[i];
                }
            }
        }

        static void RestoreLocalScales(Transform[] transforms, Vector3[] scales)
        {
            for (var i = 0; i < transforms.Length; i++)
            {
                if (transforms[i] != null)
                {
                    transforms[i].localScale = scales[i];
                }
            }
        }

        static float Smooth(float value)
        {
            value = Mathf.Clamp01(value);
            return value * value * (3f - 2f * value);
        }
    }
}
