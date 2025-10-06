using Assets.Game.Scripts.Datas;
using Assets.Game.Scripts.Signals;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.Controller
{
    public class DataController : MonoBehaviour
    {
        [SerializeField] private ProceduralGenerationInfosSO proceduralGenerationInfosSO;

        private void OnEnable()
        {
            DataSignals.Instance.onGetProceduralGenerationInfosByDifficulty += OnGetProceduralGenerationInfos;
        }

        private ProceduralGenerationInfos OnGetProceduralGenerationInfos(DifficultyTypes difficultyType) => proceduralGenerationInfosSO.proceduralGenerationInfos.Find(x=>x.DifficultyType == difficultyType);

        private void OnDisable()
        {
            DataSignals.Instance.onGetProceduralGenerationInfosByDifficulty -= OnGetProceduralGenerationInfos;
        }
    }
}