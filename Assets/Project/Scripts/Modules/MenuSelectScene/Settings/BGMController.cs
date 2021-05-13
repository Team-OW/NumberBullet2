﻿using Treevel.Common.Entities;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Treevel.Modules.MenuSelectScene.Settings
{
    public class BGMController : MonoBehaviour
    {
        private void Awake()
        {
            var slider = GetComponent<Slider>();

            // ユーザ設定が別のところで変えられた場合スライダーに反映
            UserSettings.Instance.BGMVolume.Subscribe(volume => slider.value = volume).AddTo(this);

            // BGMスライダーが変化した場合の処理
            slider.onValueChanged.AsObservable()
                .Where(v => 0.0f <= v && v <= 1.0f)
                .Subscribe(v => UserSettings.Instance.BGMVolume.Value = v)
                .AddTo(this);
        }
    }
}
