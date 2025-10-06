using Assets.Game.Scripts.Datas;
using System;
using UnityEngine;

namespace Assets.Game.Scripts.Signals
{
    public class DataSignals : MonoBehaviour
    {
        public static DataSignals Instance;

        public Func<DifficultyTypes, ProceduralGenerationInfos> onGetProceduralGenerationInfosByDifficulty;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}