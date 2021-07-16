﻿using Treevel.Common.Entities;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ReverseAttributeController : BottleAttributeControllerBase
    {
        private SpriteRenderer _spriteRenderer;

        protected override void Awake()
        {
            base.Awake();
            GamePlayDirector.Instance.StagePrepared.Subscribe(_ => _spriteRenderer.enabled = true).AddTo(compositeDisposableOnGameEnd, this);
            GamePlayDirector.Instance.GameStart.Subscribe(_ => animator.enabled = true).AddTo(this);
            GamePlayDirector.Instance.GameEnd.Subscribe(_ => animator.enabled = false).AddTo(this);
            // 描画順序の設定
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sortingOrder = EBottleAttributeType.Reverse.GetOrderInLayer();
        }

        public void Initialize(DynamicBottleController bottleController)
        {
            transform.parent = bottleController.transform;
            // y座標を中心から上に調整する
            transform.localPosition = new Vector3(0, 5f);
        }
    }
}
