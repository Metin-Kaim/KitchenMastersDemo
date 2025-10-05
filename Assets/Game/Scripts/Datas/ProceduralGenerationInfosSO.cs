using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.Datas
{
    [CreateAssetMenu(fileName = "ProceduralGenerationInfos", menuName = "KitchenMasterDemo->/new ProceduralGenerationInfosSO")]
    public class ProceduralGenerationInfosSO : ScriptableObject
    {

    }

    [Serializable]
    public struct ProceduralGenerationInfos
    {
        public DifficultyTypes DifficultyType;
        public List<Vector2Int> UsableGridSizes;
        [Range(0, 100)] public byte SpecialItemSpawnPossibility;
        [Range(0, 100)] public byte BlockSpawnPossibility;
        [Range(0, 4)] public byte MinBlockSpacing;
        [Range(1, 5)] public byte MaxStartingMatches;
    }
}