using Assets.Game.Scripts.Abstracts;
using Assets.Game.Scripts.Handlers;
using Assets.Game.Scripts.Signals;
using System;
using System.Collections.Generic;
using System.Linq;
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
            GridSignals.Instance.onGetGridCells += GetGridCells;
            GridSignals.Instance.onGetGridSize += GetGridSize;
            GridSignals.Instance.onCheckMatchesFromCell += CheckMatchesFromCell;
        }

        private void OnDisable()
        {
            GridSignals.Instance.onGetGridCells -= GetGridCells;
            GridSignals.Instance.onGetGridSize -= GetGridSize;
            GridSignals.Instance.onCheckMatchesFromCell -= CheckMatchesFromCell;
        }

        private void Start()
        {
            GenerateGrid();
        }

        private GridCellHandler[,] GetGridCells() => _gridCells;
        private Vector2Int GetGridSize() => gridSize;

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
                }
            }
        }

        // ----------------- MATCH CHECKING -----------------
        public void CheckMatchesFromCell(GridCellHandler selectedCell)
        {
            if (selectedCell == null || selectedCell.CurrentItem == null)
                return;

            List<GridCellHandler> neighbors = GetNeighborsOfSameType(selectedCell);

            if (neighbors.Count >= 4 && CheckFive(selectedCell, neighbors))
                return;

            if (neighbors.Count >= 3 && CheckFour(selectedCell, neighbors))
                return;

            if (neighbors.Count >= 2 && CheckThree(selectedCell, neighbors))
                return;
        }

        private bool CheckThree(GridCellHandler selectedCell, List<GridCellHandler> neighbors)
        {
            return CheckLineMatch(selectedCell, neighbors, 3);
        }

        private bool CheckFour(GridCellHandler selectedCell, List<GridCellHandler> neighbors)
        {
            // Önce kare kontrolü
            if (CheckSquareMatch(selectedCell, neighbors))
                return true;

            // Sonra 4’lü düz çizgi
            return CheckLineMatch(selectedCell, neighbors, 4);
        }

        private bool CheckFive(GridCellHandler selectedCell, List<GridCellHandler> neighbors)
        {
            // Önce 5’li düz çizgi
            if (CheckLineMatch(selectedCell, neighbors, 5))
                return true;

            // Sonra T ve L şekilleri
            return CheckTShapeMatch(selectedCell, neighbors) || CheckLShapeMatch(selectedCell, neighbors);
        }

        #region 🔹 Shared Helpers

        private bool CheckLineMatch(GridCellHandler selectedCell, List<GridCellHandler> neighbors, int length)
        {
            int x = selectedCell.GridPosition.x;
            int y = selectedCell.GridPosition.y;

            // --- Horizontal ---
            List<GridCellHandler> horiz = CollectLine(selectedCell, neighbors, Vector2Int.right, Vector2Int.left);
            if (horiz.Count >= length)
            {
                DestroyMatches(horiz);
                return true;
            }

            // --- Vertical ---
            List<GridCellHandler> vert = CollectLine(selectedCell, neighbors, Vector2Int.up, Vector2Int.down);
            if (vert.Count >= length)
            {
                DestroyMatches(vert);
                return true;
            }

            return false;
        }

        private List<GridCellHandler> CollectLine(GridCellHandler start, List<GridCellHandler> neighbors, Vector2Int dir1, Vector2Int dir2)
        {
            int x = start.GridPosition.x;
            int y = start.GridPosition.y;

            List<GridCellHandler> result = new() { start };

            // bir yönde
            for (int i = 1; ; i++)
            {
                var next = neighbors.FirstOrDefault(c => c.GridPosition == new Vector2Int(x + dir1.x * i, y + dir1.y * i));
                if (next != null) result.Add(next); else break;
            }
            // ters yönde
            for (int i = 1; ; i++)
            {
                var next = neighbors.FirstOrDefault(c => c.GridPosition == new Vector2Int(x + dir2.x * i, y + dir2.y * i));
                if (next != null) result.Add(next); else break;
            }

            return result;
        }

        private bool CheckSquareMatch(GridCellHandler selectedCell, List<GridCellHandler> neighbors)
        {
            int x = selectedCell.GridPosition.x;
            int y = selectedCell.GridPosition.y;

            Vector2Int[][] squareOffsets =
            {
        new [] { new Vector2Int(1,0), new Vector2Int(0,1), new Vector2Int(1,1) },   // sağ-üst
        new [] { new Vector2Int(-1,0), new Vector2Int(0,1), new Vector2Int(-1,1) }, // sol-üst
        new [] { new Vector2Int(1,0), new Vector2Int(0,-1), new Vector2Int(1,-1) }, // sağ-alt
        new [] { new Vector2Int(-1,0), new Vector2Int(0,-1), new Vector2Int(-1,-1)} // sol-alt
    };

            foreach (var offsets in squareOffsets)
            {
                List<GridCellHandler> square = new() { selectedCell };
                bool valid = true;

                foreach (var off in offsets)
                {
                    var cell = neighbors.FirstOrDefault(c => c.GridPosition == new Vector2Int(x + off.x, y + off.y));
                    if (cell != null) square.Add(cell);
                    else { valid = false; break; }
                }

                if (valid)
                {
                    DestroyMatches(square);
                    return true;
                }
            }
            return false;
        }

        private bool CheckTShapeMatch(GridCellHandler selectedCell, List<GridCellHandler> neighbors)
        {
            int x = selectedCell.GridPosition.x;
            int y = selectedCell.GridPosition.y;

            // Yukarı T
            if (Has(neighbors, x, y - 1, x, y - 2) && Has(neighbors, x - 1, y, x + 1, y))
                return DestroyAndReturn(selectedCell, neighbors, new (int, int)[] { (0, -1), (0, -2), (-1, 0), (1, 0) });

            // Aşağı T
            if (Has(neighbors, x, y + 1, x, y + 2) && Has(neighbors, x - 1, y, x + 1, y))
                return DestroyAndReturn(selectedCell, neighbors, new (int, int)[] { (0, 1), (0, 2), (-1, 0), (1, 0) });

            // Sol T
            if (Has(neighbors, x - 1, y, x - 2, y) && Has(neighbors, x, y - 1, x, y + 1))
                return DestroyAndReturn(selectedCell, neighbors, new (int, int)[] { (-1, 0), (-2, 0), (0, -1), (0, 1) });

            // Sağ T
            if (Has(neighbors, x + 1, y, x + 2, y) && Has(neighbors, x, y - 1, x, y + 1))
                return DestroyAndReturn(selectedCell, neighbors, new (int, int)[] { (1, 0), (2, 0), (0, -1), (0, 1) });

            return false;
        }

        private bool CheckLShapeMatch(GridCellHandler selectedCell, List<GridCellHandler> neighbors)
        {
            int x = selectedCell.GridPosition.x;
            int y = selectedCell.GridPosition.y;

            // Sol + Yukarı
            if (Has(neighbors, x - 1, y, x - 2, y, x, y + 1, x, y + 2))
                return DestroyAndReturn(selectedCell, neighbors, new (int, int)[] { (-1, 0), (-2, 0), (0, 1), (0, 2) });

            // Sol + Aşağı
            if (Has(neighbors, x - 1, y, x - 2, y, x, y - 1, x, y - 2))
                return DestroyAndReturn(selectedCell, neighbors, new (int, int)[] { (-1, 0), (-2, 0), (0, -1), (0, -2) });

            // Sağ + Yukarı
            if (Has(neighbors, x + 1, y, x + 2, y, x, y + 1, x, y + 2))
                return DestroyAndReturn(selectedCell, neighbors, new (int, int)[] { (1, 0), (2, 0), (0, 1), (0, 2) });

            // Sağ + Aşağı
            if (Has(neighbors, x + 1, y, x + 2, y, x, y - 1, x, y - 2))
                return DestroyAndReturn(selectedCell, neighbors, new (int, int)[] { (1, 0), (2, 0), (0, -1), (0, -2) });

            return false;
        }

        private bool Has(List<GridCellHandler> neighbors, params int[] coords)
        {
            for (int i = 0; i < coords.Length; i += 2)
                if (!neighbors.Exists(c => c.GridPosition == new Vector2Int(coords[i], coords[i + 1])))
                    return false;
            return true;
        }

        private bool DestroyAndReturn(GridCellHandler start, List<GridCellHandler> neighbors, (int dx, int dy)[] offsets)
        {
            List<GridCellHandler> cells = new() { start };
            int x = start.GridPosition.x;
            int y = start.GridPosition.y;

            foreach (var (dx, dy) in offsets)
                cells.Add(neighbors.Find(c => c.GridPosition == new Vector2Int(x + dx, y + dy)));

            DestroyMatches(cells);
            return true;
        }

        private void DestroyMatches(List<GridCellHandler> matchedCells)
        {
            if (matchedCells == null || matchedCells.Count == 0)
                return;

            foreach (var cell in matchedCells)
            {
                if (cell == null || cell.CurrentItem == null)
                    continue;

                if (cell.CurrentItem is MonoBehaviour mb)
                {
                    Destroy(mb.gameObject);
                }

                cell.CurrentItem = null;
            }
        }

        private List<GridCellHandler> GetNeighborsOfSameType(GridCellHandler startCell)
        {
            List<GridCellHandler> connectedCells = new();

            if (startCell == null || startCell.CurrentItem == null)
                return connectedCells;

            ItemTypes targetType = startCell.CurrentItem.ItemType;
            Queue<GridCellHandler> queue = new();
            queue.Enqueue(startCell);

            while (queue.Count > 0)
            {
                GridCellHandler cell = queue.Dequeue();

                if (cell.IsChecked || cell.CurrentItem == null)
                    continue;

                if (cell.CurrentItem.ItemType != targetType)
                    continue;

                cell.IsChecked = true;
                connectedCells.Add(cell);

                int x = cell.GridPosition.x;
                int y = cell.GridPosition.y;

                // Komşuluk (4 yönlü)
                int[,] directions = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };

                for (int i = 0; i < directions.GetLength(0); i++)
                {
                    int nx = x + directions[i, 0];
                    int ny = y + directions[i, 1];

                    if (nx >= 0 && nx < gridSize.x && ny >= 0 && ny < gridSize.y)
                    {
                        var neighbor = _gridCells[nx, ny];
                        if (neighbor != null && !neighbor.IsChecked &&
                            neighbor.CurrentItem != null && neighbor.CurrentItem.ItemType == targetType)
                        {
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }

            // Tüm hücrelerdeki IsChecked'i resetle
            foreach (var cell in connectedCells)
                cell.IsChecked = false;

            // Başlangıç hücresini çıkartıyoruz çünkü neighbors sadece komşuları temsil etmeli
            connectedCells.Remove(startCell);

            return connectedCells;
        }

        #endregion

    }
}
