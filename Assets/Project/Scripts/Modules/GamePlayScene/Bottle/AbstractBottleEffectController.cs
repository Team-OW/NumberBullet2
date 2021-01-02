﻿using System;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Bottle
{
    public class AbstractBottleEffectController : MonoBehaviour
    {
        /// <summary>
        /// 購読解除クラス
        /// </summary>
        protected CompositeDisposable eventDisposable = new CompositeDisposable();

        protected void DisposeEvent()
        {
            eventDisposable.Dispose();
        }
    }
}
