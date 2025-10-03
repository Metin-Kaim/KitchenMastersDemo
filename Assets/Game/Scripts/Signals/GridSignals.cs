using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Game.Scripts.Signals
{
    public class GridSignals : MonoBehaviour
    {
        public static GridSignals Instance;

        public Func<GridCellHandler[,]> onGetGridCells;
        public Func<Vector2Int> onGetGridSize;
        public UnityAction onCheckTheGridToMatch;
        public UnityAction<GridCellHandler> onCheckMatchesFromCell;
        internal UnityAction<GridCellHandler> onSpawnNewItems;
        public UnityAction<List<GridCellHandler>, bool> onDestroyMatches;

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