using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.Datas
{
    [Serializable]
    public class LevelSaveData
    {
        public DifficultyTypes DifficultyType;
        public Vector2Int GridSize;
        public List<CellInfo> CellInfos;
    }
    [Serializable]
    public class CellInfo
    {
        public Vector2Int Position;
        public ItemTypes ItemTypeOfInsideItem;
        public bool IsCheckable;
        public bool IsLocked;
    }
}