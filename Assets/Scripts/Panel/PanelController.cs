﻿using System;
using Tile;
using TouchScript.Gestures;
using UnityEngine;

namespace Panel
{
    public class PanelController : MonoBehaviour
    {
        private void Start()
        {
            // 当たり判定をパネルサイズと同等にする
            Vector2 panelSize = transform.localScale * 2f;
            BoxCollider2D collider = GetComponent<BoxCollider2D> ();
            collider.size = panelSize;
        }

        private void OnEnable()
        {
            GetComponent<FlickGesture>().StateChanged += HandleFlick;
            // フリックの検知感度を変えたい際に変更可能
            GetComponent<FlickGesture>().MinDistance = 0.5f;
            GetComponent<FlickGesture>().FlickTime = 0.5f;
        }

        private void OnDisable()
        {
            GetComponent<FlickGesture>().Flicked -= HandleFlick;
        }

        private void HandleFlick(object sender, System.EventArgs e)
        {
            FlickGesture gesture = sender as FlickGesture;

            if (gesture.State != FlickGesture.GestureState.Recognized) return;

            // 親タイルオブジェクトのスクリプトを取得
            TileController parentTile = transform.parent.gameObject.GetComponent<TileController>();
            // フリック方向
            var x = gesture.ScreenFlickVector.x;
            var y = gesture.ScreenFlickVector.y;

            // 方向検知に加えて，上下と左右の変化量を比べることで，検知精度をあげる
            if (x > 0 && Math.Abs(x) >= Math.Abs(y))
            {
                // 右
                GameObject rightTile = parentTile.rightTile;
                Move(rightTile);
            }
            else if (x < 0 && Math.Abs(x) >= Math.Abs(y))
            {
                // 左
                GameObject leftTile = parentTile.leftTile;
                Move(leftTile);
            }
            else if (y > 0 && Math.Abs(y) >= Math.Abs(x))
            {
                // 上
                GameObject upperTile = parentTile.upperTile;
                Move(upperTile);
            }
            else if (y < 0 && Math.Abs(y) >= Math.Abs(x))
            {
                // 下
                GameObject lowerTile = parentTile.lowerTile;
                Move(lowerTile);
            }
        }

        private void Move(GameObject targetTile)
        {
            // 移動先にタイルがなければ何もしない
            if (targetTile == null) return;
            // 移動先のタイルに子パネルがあれば何もしない
            if (targetTile.transform.childCount != 0) return;
            // 親タイルの更新
            transform.parent = targetTile.transform;
            // 移動
            transform.position = Vector2.MoveTowards(transform.position, targetTile.transform.position, 2.5f);
        }
    }
}
