using Assets.Game.Scripts.Datas;
using Assets.Game.Scripts.Signals;
using UnityEngine;

namespace Assets.Game.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        private void OnEnable()
        {
            GameSignals.Instance.onSaveGame += ExecuteLevelAsJSONFormat;
        }
        private void OnDisable()
        {
            GameSignals.Instance.onSaveGame -= ExecuteLevelAsJSONFormat;
        }

        public void ExecuteLevelAsJSONFormat(LevelSaveData saveData)
        {
            string jsonData = JsonUtility.ToJson(saveData, true);
            print(jsonData);
        }
    }
}