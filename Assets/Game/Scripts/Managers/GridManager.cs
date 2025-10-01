using Assets.Game.Scripts.Abstracts;
using Assets.Game.Scripts.Handlers;
using Assets.Game.Scripts.Signals;
using System;
using UnityEngine;

namespace Assets.Game.Scripts.Managers
{
    public class GridManager : MonoBehaviour
    {
        [Header("Prefabs and Containers")]
        [SerializeField] private GridCellHandler gridCellPrefab;
        [SerializeField] private Transform gridCellsContainer;
        [SerializeField] private GameObject[] candies;
        [Header("Grid Settings")]
        [SerializeField] private Vector2Int gridSize = new(8, 8);

        private GridCellHandler[,] _gridCells;

        private void OnEnable()
        {
            GridSignals.Instance.onGetGridCells += () => _gridCells;
            GridSignals.Instance.onGetGridSize += () => gridSize;
        }
        private void OnDisable()
        {
            GridSignals.Instance.onGetGridCells -= () => _gridCells;
            GridSignals.Instance.onGetGridSize -= () => gridSize;
        }

        private void Start()
        {
            GenerateGrid();
        }
        private void GenerateGrid()
        {
            _gridCells = new GridCellHandler[gridSize.x, gridSize.y];

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Vector2 position = new(x, y);
                    GridCellHandler cell = Instantiate(gridCellPrefab, position, Quaternion.identity, gridCellsContainer);
                    cell.name = $"Cell_{x}_{y}";
                    cell.GridPosition = new(x, y);
                    _gridCells[x, y] = cell;
                    // Optionally, set cell properties here
                }
            }

            PopulateGridWithCandies();
        }

        private void PopulateGridWithCandies()
        {
            System.Random random = new();
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    int randomIndex = random.Next(candies.Length);
                    GameObject candy = Instantiate(candies[randomIndex]);
                    candy.transform.SetParent(_gridCells[x, y].transform);
                    candy.transform.localPosition = Vector3.zero;
                    candy.name += $"_({x},{y})";

                    _gridCells[x, y].CurrentItem = candy.GetComponent<IItem>();
                    // Optionally, set candy properties here
                }
            }

        }
    }
}