﻿using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Treevel.Common.Entities.GameDatas
{
    [System.Serializable]
    public class BottleData
    {
        public EBottleType type;
        [Range(1, 15)] public int initPos;
        [Range(1, 15)] public short targetPos;
        public AssetReferenceSprite bottleSprite;
        public AssetReferenceSprite targetTileSprite;
        public short life;
        public bool isSelfish;
        public bool isDark;
    }
}
