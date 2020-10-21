﻿using Treevel.Common.Utils;
using Treevel.Common.Patterns.Singleton;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene
{
    public class GameWindowController : SingletonObject<GameWindowController>
    {
        /// <summary>
        /// ゲーム画面以外を埋める背景
        /// </summary>
        [SerializeField] private GameObject _background;

        /// <summary>
        /// ゲーム画面と同じ大きさの背景用マスク
        /// </summary>
        [SerializeField] private GameObject _backgroundMask;

        private float gameWindowWidth = Constants.WindowSize.WIDTH;

        /// <summary>
        /// 9:16のゲーム領域を覆うPanel
        /// </summary>
        [SerializeField] private GameObject _gameAreaPanel;

        private void Awake()
        {
            UnifyDisplay();
        }

        /// <summary>
        /// ゲーム画面のアスペクト比を統一する
        /// </summary>
        /// Bug: ゲーム画面遷移時にカメラ範囲が狭くなることがある
        private void UnifyDisplay()
        {
            // 想定するデバイスのアスペクト比
            const float targetRatio = Constants.WindowSize.WIDTH / Constants.WindowSize.HEIGHT;
            // 実際のデバイスのアスペクト比
            var currentRatio = (float)Screen.width / Screen.height;
            // 許容するアスペクト比の誤差
            const float aspectRatioError = 0.001f;

            if (Mathf.Abs(currentRatio - targetRatio) <= aspectRatioError) return;

            // ゲーム盤面以外を埋める背景画像を表示する
            var backgroundSize = _background.GetComponent<SpriteRenderer>().size;
            var originalWidth = backgroundSize.x;
            var originalHeight = backgroundSize.y;
            var rect = _gameAreaPanel.GetComponent<RectTransform>();
            var camera = Camera.main;
            if (currentRatio > targetRatio + aspectRatioError) {
                // 横長のデバイスの場合
                var ratio = targetRatio / currentRatio;
                var rectX = (1 - ratio) / 2f;
                _backgroundMask.transform.localScale = new Vector2(Constants.WindowSize.WIDTH / originalWidth, Constants.WindowSize.HEIGHT / originalHeight);
                _background.transform.localScale = new Vector2(Constants.WindowSize.WIDTH / originalWidth / ratio, Constants.WindowSize.HEIGHT / originalHeight);
                // GameAreaPanelの大きさを変更
                rect.anchorMin = new Vector2(Mathf.Max(rect.anchorMin.x, rectX), rect.anchorMin.y);
                rect.anchorMax = new Vector2(Mathf.Min(rect.anchorMax.x, rectX + ratio), rect.anchorMax.y);
            } else if (currentRatio < targetRatio - aspectRatioError) {
                // 縦長のデバイスの場合
                var ratio = currentRatio / targetRatio;
                var rectY = (1 - ratio) / 2f;
                _backgroundMask.transform.localScale = new Vector2(Constants.WindowSize.WIDTH / originalWidth * ratio, Constants.WindowSize.HEIGHT / originalHeight * ratio);
                _background.transform.localScale = new Vector2(Constants.WindowSize.WIDTH / originalWidth * ratio, Constants.WindowSize.HEIGHT / originalHeight);
                rect.anchorMin = new Vector2(rect.anchorMin.x, Mathf.Max(rect.anchorMin.y, rectY));
                rect.anchorMax = new Vector2(rect.anchorMax.x, Mathf.Min(rect.anchorMax.y, rectY + ratio));
                // ゲーム画面の横幅を更新
                gameWindowWidth *= ratio;
            }
            _backgroundMask.GetComponent<SpriteMask>().enabled = true;
            _background.GetComponent<SpriteRenderer>().enabled = true;
        }

        /// <summary>
        /// ゲーム画面の横幅を取得する
        /// </summary>
        /// <returns></returns>
        public float GetGameWindowWidth()
        {
            return gameWindowWidth;
        }

        /// <summary>
        /// タイルの横幅を取得する
        /// </summary>
        /// <returns></returns>
        public float GetTileWidth()
        {
            return gameWindowWidth * Constants.TileRatioToWindowWidth.WIDTH_RATIO;
        }

        /// <summary>
        /// タイルの縦幅を取得する
        /// </summary>
        /// <returns></returns>
        public float GetTileHeight()
        {
            return gameWindowWidth * Constants.TileRatioToWindowWidth.HEIGHT_RATIO;
        }
    }
}
