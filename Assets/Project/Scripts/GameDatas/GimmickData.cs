﻿using System.Collections.Generic;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GameDatas
{
    [System.Serializable]
    public class GimmickData
    {
        public bool loop;
        public float appearTime;
        public float interval;
        public EGimmickType type;

        public int line;

        public ECartridgeDirection direction;

        /// <summary>
        /// ＜竜巻＞攻撃方向リスト
        /// </summary>
        public List<ECartridgeDirection> targetDirections;

        /// <summary>
        /// ＜竜巻＞攻撃行列リスト
        /// </summary>
        public List<int> targetLines;

        /// <summary>
        /// ＜竜巻＞ランダム攻撃方向重みリスト
        /// </summary>
        public List<int> randomDirection;

        /// <summary>
        /// ＜竜巻＞ランダム攻撃行重みリスト
        /// </summary>
        public List<int> randomRow;

        /// <summary>
        /// ＜竜巻＞ランダム攻撃列重みリスト
        /// </summary>
        public List<int> randomColumn;

        public List<int> turnLines;

        [Range(1, 15)] public List<int> aimingBottles;

        public List<int> randomCartridgeDirection;
        public List<int> randomLine;


        public List<int> randomAttackableBottles;
    }
}