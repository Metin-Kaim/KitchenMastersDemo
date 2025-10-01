using Assets.Game.Scripts.Handlers;
using System;
using UnityEngine;

namespace Assets.Game.Scripts.Signals
{
    public class GridSignals : MonoBehaviour
    {
        public static GridSignals Instance;

        public Func<GridCellHandler[,]> onGetGridCells;
        public Func<Vector2Int> onGetGridSize;

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