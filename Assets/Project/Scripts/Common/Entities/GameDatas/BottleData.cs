﻿using System;
using UnityEngine;

namespace Treevel.Common.Entities.GameDatas
{
    [Serializable]
    public class BottleData
    {
        public EBottleType type;
        [Range(1, 15)] public short initPos;
        public EGoalColor goalColor;
        public short life;
        public bool isGhost;
        public bool isDark;
        public bool isReverse;
    }
}
