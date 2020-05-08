﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.UIComponents
{
    public class FadeObject2D : MonoBehaviour
    {
        [SerializeField, Tooltip("fade duration in second")] private float _duration = 0.8f;

        private List<CanvasRenderer> _renderers = new List<CanvasRenderer>();

        private void Awake()
        {
            GetComponentsInChildren<CanvasRenderer>(true, _renderers);
        }

        private void OnEnable()
        {
            if (_renderers == null || _renderers.Count == 0)
                return;

            _renderers.ForEach(r => r.SetAlpha(0));
            StartCoroutine(FadeIn());
        }

        private IEnumerator FadeIn()
        {
            var elapsed = 0.0f;
            while (elapsed < _duration) {
                elapsed += Time.deltaTime;
                var alpha = Mathf.Lerp(0, 1, elapsed / _duration);
                _renderers.ForEach(r => r.SetAlpha(alpha));
                yield return null;
            }
        }
    }
}
