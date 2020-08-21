﻿using System;
using Project.Scripts.GamePlayScene.Bottle;
using Project.Scripts.GamePlayScene.Tile;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils;
using UnityEngine;
using Project.Scripts.GamePlayScene.Gimmick;

namespace Project.Scripts.GamePlayScene
{
    public static class StageGenerator
    {
        /// <summary>
        /// ステージ作成が完了したかどうかのフラグ
        /// </summary>
        public static bool CreatedFinished
        {
            get;
            private set;
        }

        /// <summary>
        /// ステージを作成する
        /// </summary>
        /// <param name="treeId"> 木のID </param>
        /// <param name="stageNumber"> ステージ番号 </param>
        /// <exception cref="NotImplementedException"> 実装されていないステージ id を指定した場合 </exception>
        public static async void CreateStages(ETreeId treeId, int stageNumber)
        {
            CreatedFinished = false;

            var tileGenerator = TileGenerator.Instance;

            // ステージデータ読み込む
            var stageData = GameDataBase.GetStage(treeId, stageNumber);
            if (stageData != null) {
                // タイル生成
                tileGenerator.CreateTiles(stageData.TileDatas);

                // ボトル生成
                BottleGenerator.Instance.CreateBottles(stageData.BottleDatas);

                // ギミック生成
                GimmickGenerator.Instance.Initialize(stageData.GimmickDatas);
            } else {
                // 存在しないステージ
                Debug.LogError("Unable to create a stage whose stageId is " + stageNumber.ToString() + ".");
            }

            CreatedFinished = true;
        }
    }
}
