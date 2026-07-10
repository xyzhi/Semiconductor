using System.Collections;
using UnityEngine;

namespace SemiconductorTeaching
{
    public sealed class PNJunctionTeachingAnimator : MonoBehaviour
    {
        public bool playOnEnable = true;
        public float cycleDuration = 2.8f;
        public float pauseDuration = 0.45f;
        public float carrierMoveDistance = 0.055f;
        public float ionPulseScale = 1.45f;

        Transform[] electrons;
        Transform[] holes;
        Transform[] fixedIons;
        Vector3[] electronStartPositions;
        Vector3[] holeStartPositions;
        Vector3[] ionStartScales;
        Coroutine animationRoutine;

        void Awake()
        {
            electrons = FindChildren("NPart/Electrons");
            holes = FindChildren("PPart/Holes");
            fixedIons = Combine(
                FindChildren("MiddlePart/FixedNegativeIons"),
                FindChildren("MiddlePart/FixedPositiveIons"));

            electronStartPositions = ReadLocalPositions(electrons);
            holeStartPositions = ReadLocalPositions(holes);
            ionStartScales = ReadLocalScales(fixedIons);
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
            if (animationRoutine != null)
            {
                StopCoroutine(animationRoutine);
            }

            animationRoutine = StartCoroutine(Animate());
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

        IEnumerator Animate()
        {
            while (true)
            {
                var elapsed = 0f;
                while (elapsed < cycleDuration)
                {
                    elapsed += Time.deltaTime;
                    var t = Mathf.Clamp01(elapsed / cycleDuration);
                    var wave = Mathf.Sin(t * Mathf.PI);

                    MoveCarriers(electrons, electronStartPositions, Vector3.left, wave);
                    MoveCarriers(holes, holeStartPositions, Vector3.right, wave);
                    PulseIons(wave);

                    yield return null;
                }

                ResetState();
                if (pauseDuration > 0f)
                {
                    yield return new WaitForSeconds(pauseDuration);
                }
            }
        }

        void MoveCarriers(Transform[] carriers, Vector3[] startPositions, Vector3 direction, float wave)
        {
            for (var i = 0; i < carriers.Length; i++)
            {
                if (carriers[i] != null)
                {
                    carriers[i].localPosition = startPositions[i] + direction * carrierMoveDistance * wave;
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

        void ResetState()
        {
            RestoreLocalPositions(electrons, electronStartPositions);
            RestoreLocalPositions(holes, holeStartPositions);
            RestoreLocalScales(fixedIons, ionStartScales);
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
    }
}
