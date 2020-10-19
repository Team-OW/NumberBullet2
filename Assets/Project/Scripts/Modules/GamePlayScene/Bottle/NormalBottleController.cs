﻿using SpriteGlow;
using System;
using TouchScript.Gestures;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using Treevel.Modules.GamePlayScene.Tile;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    [RequireComponent(typeof(PostProcessVolume))]
    [RequireComponent(typeof(SpriteGlowEffect))]
    [RequireComponent(typeof(LongPressGesture))]
    public class NormalBottleController : DynamicBottleController
    {
        private LongPressGesture _longPressGesture;

        /// <summary>
        /// 目標位置
        /// </summary>
        private int _targetPos;

        /// <summary>
        /// 光らせるエフェクト
        /// </summary>
        private SpriteGlowEffect _spriteGlowEffect;

        /// <summary>
        /// 長押しされたときの処理
        /// </summary>
        public event Action OnLongPressed
        {
            add => _onLongPressedInvoker += value;
            remove => _onLongPressedInvoker -= value;
        }
        private event Action _onLongPressedInvoker;

        protected override void Awake()
        {
            base.Awake();
            _longPressGesture = GetComponent<LongPressGesture>();
            _longPressGesture.TimeToPress = 0.15f;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            OnEnterTile += HandleOnEnterTile;
            OnExitTile += HandleOnExitTile;
            _longPressGesture.LongPressed += HandleOnLongPressed;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            OnEnterTile -= HandleOnEnterTile;
            OnExitTile -= HandleOnExitTile;
            _longPressGesture.LongPressed -= HandleOnLongPressed;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="bottleData">ボトルデータ</param>
        public override async void Initialize(BottleData bottleData)
        {
            _spriteGlowEffect = GetComponent<SpriteGlowEffect>();
            _spriteGlowEffect.enabled = false;

            // parse data
            var finalPos = bottleData.targetPos;
            _targetPos = finalPos;
            var targetTileSprite = AddressableAssetManager.GetAsset<Sprite>(bottleData.targetTileSprite);

            base.Initialize(bottleData);

            // set handler
            var lifeEffect = await AddressableAssetManager.Instantiate(Constants.Address.LIFE_EFFECT_PREFAB).Task;
            lifeEffect.GetComponent<LifeEffectController>().Initialize(this, bottleData.life);
            if (bottleData.isDark) {
                var darkEffect = await AddressableAssetManager.Instantiate(Constants.Address.DARK_EFFECT_PREFAB).Task;
                darkEffect.GetComponent<DarkEffectController>().Initialize(this);
            }

            #if UNITY_EDITOR
            name = Constants.BottleName.NORMAL_BOTTLE + Id.ToString();
            #endif

            // 目標とするタイルのスプライトを設定
            var finalTile = BoardManager.Instance.GetTile(finalPos);
            finalTile.GetComponent<NormalTileController>().SetSprite(targetTileSprite);
        }

        private void HandleOnEnterTile(GameObject targetTile)
        {
            if (IsSuccess()) {
                DoWhenSuccess();
            }
        }

        private void HandleOnExitTile(GameObject targetTile)
        {
            _spriteGlowEffect.enabled = false;
        }

        private void HandleOnLongPressed(object sender, EventArgs e)
        {
            _onLongPressedInvoker?.Invoke();
        }

        /// <summary>
        /// 目標タイルにいる時の処理
        /// </summary>
        private void DoWhenSuccess()
        {
            // 光らせる
            _spriteGlowEffect.enabled = true;
            // ステージの成功判定
            GamePlayDirector.Instance.CheckClear();
        }

        /// <summary>
        /// 目標タイルにいるかどうか
        /// </summary>
        /// <returns></returns>
        public bool IsSuccess()
        {
            return _targetPos != 0 && BoardManager.Instance.GetBottlePos(this) == _targetPos;
        }
    }
}
