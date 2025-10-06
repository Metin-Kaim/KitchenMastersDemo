using Assets.Game.Scripts.Datas;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Game.Scripts.Signals
{
    public class GameSignals : MonoBehaviour
    {
        public static GameSignals Instance;

        public UnityAction<LevelSaveData> onSaveGame;

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