﻿using UnityEngine;

namespace Project.Scripts.GamePlayScene.Bottle
{
    /// <summary>
    /// 成功判定を処理するインターフェイス
    /// 成功判定させたいボトルクラスに実装させて、タイルに移動した際成功判定を発火する
    /// </summary>
    public interface IBottleSuccessHandler
    {
        /// <summary>
        /// ボトルがゴールとなるタイルに載った時の挙動
        /// </summary>
        void DoWhenSuccess();

        /// <summary>
        /// 該当ボトルが成功状態かどうか
        /// </summary>
        /// <returns></returns>
        bool IsSuccess();
    }
}
