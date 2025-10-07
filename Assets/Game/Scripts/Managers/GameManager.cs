using Assets.Game.Scripts.Datas;
using Assets.Game.Scripts.Signals;
using System;
using UnityEngine;

namespace Assets.Game.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private DifficultyTypes difficultyType;

        private void OnEnable()
        {
            GameSignals.Instance.onSaveGame += ExecuteLevelAsJSONFormat;
            GameSignals.Instance.onGetDifficultyType += OnGetDifficultyType;
        }

        private DifficultyTypes OnGetDifficultyType() => difficultyType;

        private void OnDisable()
        {
            GameSignals.Instance.onSaveGame -= ExecuteLevelAsJSONFormat;
            GameSignals.Instance.onGetDifficultyType -= OnGetDifficultyType;
        }

        private void Awake()
        {
            Array array = Enum.GetValues(typeof(DifficultyTypes));
            difficultyType = (DifficultyTypes)array.GetValue(UnityEngine.Random.Range(0, array.Length));

            print("Selected Difficulty Type: " + difficultyType);
        }

        public void ExecuteLevelAsJSONFormat(LevelSaveData saveData)
        {
            string jsonData = JsonUtility.ToJson(saveData, true);
            print(jsonData);
        }
    }
}