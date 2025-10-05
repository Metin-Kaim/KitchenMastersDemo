using Assets.Game.Scripts.Abstracts;
using Assets.Game.Scripts.Handlers;
using Assets.Game.Scripts.Signals;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Game.Scripts.Managers
{
    public class GridManager : MonoBehaviour
    {
        #region Fields & Inspector

        [Header("Prefabs and Containers")]
        [SerializeField] private GridCellHandler gridCellPrefab;
        [SerializeField] private Transform gridCellsContainer;
        [SerializeField] private CandyHandler[] candies;
        [SerializeField] private AbsBlock[] blocks;
        [SerializeField] private AbsBlock[] hibritBlocks;
        [SerializeField] private AbsSpecial[] specialItems;

        [Header("Grid Settings")]
        [SerializeField] private Vector2Int gridSize = new(8, 8);

        private GridCellHandler[,] _gridCells;

        #endregion

        #region Unity Lifecycle

        private void OnEnable()
        {
            GridSignals.Instance.onGetGridCells += GetGridCells;
            GridSignals.Instance.onGetGridSize += GetGridSize;
            GridSignals.Instance.onCheckMatchesFromCell += CheckMatchesFromCell;
            GridSignals.Instance.onSpawnNewItems += OnSpawnNewItems;
            GridSignals.Instance.onDestroyMatches += DestroyMatches;
        }

        private void OnDisable()
        {
            GridSignals.Instance.onGetGridCells -= GetGridCells;
            GridSignals.Instance.onGetGridSize -= GetGridSize;
            GridSignals.Instance.onCheckMatchesFromCell -= CheckMatchesFromCell;
            GridSignals.Instance.onSpawnNewItems -= OnSpawnNewItems;
            GridSignals.Instance.onDestroyMatches -= DestroyMatches;
        }

        private void Start() => GenerateGrid();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                Debug.Break();
        }

        #endregion

        #region Grid Accessors

        private GridCellHandler[,] GetGridCells() => _gridCells;
        private Vector2Int GetGridSize() => gridSize;

        #endregion

        #region Grid Generation

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

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if ((x == 1 && y == 3) || (x == 3 && y == 2) || (x == 1 && y == 2) || (x == 5 && y == 2))
                    {
                        int blockIndex = Random.Range(0, hibritBlocks.Length);
                        AbsBlock block = Instantiate(hibritBlocks[blockIndex], _gridCells[x, y].transform);
                        block.transform.localPosition = Vector3.zero;
                        block.name += $"_({x},{y})";
                        _gridCells[x, y].CurrentItem = block.GetComponent<IItem>();
                        _gridCells[x, y].IsCheckable = false;
                        (block as IItem).CurrentCell = _gridCells[x, y];
                    }
                }
            }

            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    int blockIndex = Random.Range(0, blocks.Length);
                    var block = Instantiate(blocks[blockIndex], _gridCells[x, y].transform);
                    block.transform.localPosition = Vector3.zero;
                    block.name += $"_({x},{y})";
                    _gridCells[x, y].CurrentItem = block.GetComponent<IItem>();
                    _gridCells[x, y].IsCheckable = false;
                    _gridCells[x, y].IsLocked = true;
                    (block as IItem).CurrentCell = _gridCells[x, y];
                }
            }

            int specialIndex = Random.Range(0, 1);
            var special = Instantiate(specialItems[specialIndex], _gridCells[4, 4].transform);
            special.transform.localPosition = Vector3.zero;
            special.name += $"_(4,4)";
            _gridCells[4, 4].CurrentItem = special.GetComponent<IItem>();
            _gridCells[4, 4].IsCheckable = false;
            _gridCells[4, 4].IsLocked = false;
            (special as IItem).CurrentCell = _gridCells[4, 4];


            int specialIndex2 = Random.Range(0, 1);
            var special2 = Instantiate(specialItems[specialIndex2], _gridCells[4,3].transform);
            special2.transform.localPosition = Vector3.zero;
            special2.name += $"_[4,3]";
            _gridCells[4,3].CurrentItem = special2.GetComponent<IItem>();
            _gridCells[4,3].IsCheckable = false;
            _gridCells[4,3].IsLocked = false;
            (special2 as IItem).CurrentCell = _gridCells[4,3];


            PopulateGridWithCandies();
        }

        private void PopulateGridWithCandies()
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if (_gridCells[x, y].CurrentItem != null) continue;

                    int randomIndex = Random.Range(0, candies.Length);
                    var candy = Instantiate(candies[randomIndex], _gridCells[x, y].transform);
                    candy.transform.localPosition = Vector3.zero;
                    candy.name += $"_({x},{y})";
                    _gridCells[x, y].CurrentItem = candy.GetComponent<IItem>();
                    candy.CurrentCell = _gridCells[x, y];
                    _gridCells[x, y].IsCheckable = true;
                    _gridCells[x, y].IsLocked = false;
                }
            }
        }

        #endregion

        #region Spawn New Items

        public void OnSpawnNewItems(GridCellHandler emptyCell)
        {
            int column = emptyCell.GridPosition.x;
            int startY = emptyCell.GridPosition.y;

            for (int y = startY; y < gridSize.y; y++)
            {
                var cell = _gridCells[column, y];
                if (cell.CurrentItem != null) continue;

                int randomIndex = Random.Range(0, candies.Length);
                var candy = Instantiate(candies[randomIndex], cell.transform);
                candy.transform.localPosition = Vector3.up * (gridSize.y - y + 1); // yukarıdan düşecek
                candy.name += $"_({cell.GridPosition.x},{cell.GridPosition.y})";
                cell.CurrentItem = candy.GetComponent<IItem>();
                candy.CurrentCell = cell;
                cell.IsCheckable = true;
                cell.IsLocked = false;

                if (candy.TryGetComponent(out IMovable movable))
                    movable.FallToTheCell(cell);
            }
        }

        #endregion

        #region Match Checking

        public void CheckMatchesFromCell(GridCellHandler selectedCell)
        {
            if (selectedCell?.CurrentItem == null) return;

            List<GridCellHandler> neighbors = GetNeighborsOfSameType(selectedCell);

            if (neighbors.Count >= 4 && CheckFive(selectedCell, neighbors)) return;
            if (neighbors.Count >= 3 && CheckFour(selectedCell, neighbors)) return;
            if (neighbors.Count >= 2 && CheckThree(selectedCell, neighbors)) return;
        }

        private bool CheckThree(GridCellHandler cell, List<GridCellHandler> neighbors) =>
            CheckLineMatch(cell, neighbors, 3);

        private bool CheckFour(GridCellHandler cell, List<GridCellHandler> neighbors) =>
            CheckSquareMatch(cell, neighbors) || CheckLineMatch(cell, neighbors, 4);

        private bool CheckFive(GridCellHandler cell, List<GridCellHandler> neighbors) =>
            CheckLineMatch(cell, neighbors, 5) ||
            CheckTShapeMatch(cell, neighbors) ||
            CheckLShapeMatch(cell, neighbors);

        #endregion

        #region Match Helpers

        private bool CheckLineMatch(GridCellHandler cell, List<GridCellHandler> neighbors, int length)
        {
            List<GridCellHandler> horiz = CollectLine(cell, neighbors, Vector2Int.right, Vector2Int.left);
            if (horiz.Count >= length)
            {
                DestroyMatches(horiz);
                return true;
            }

            List<GridCellHandler> vert = CollectLine(cell, neighbors, Vector2Int.up, Vector2Int.down);
            if (vert.Count >= length)
            {
                DestroyMatches(vert);
                return true;
            }

            return false;
        }

        private List<GridCellHandler> CollectLine(GridCellHandler start, List<GridCellHandler> neighbors, Vector2Int dir1, Vector2Int dir2)
        {
            List<GridCellHandler> result = new() { start };
            AddDirection(neighbors, result, start.GridPosition, dir1);
            AddDirection(neighbors, result, start.GridPosition, dir2);
            return result;
        }

        private void AddDirection(List<GridCellHandler> neighbors, List<GridCellHandler> result, Vector2Int startPos, Vector2Int dir)
        {
            for (int i = 1; ; i++)
            {
                var nextCell = neighbors.FirstOrDefault(c => c.GridPosition == startPos + dir * i);
                if (nextCell != null)
                {
                    if (!nextCell.IsLocked)
                        result.Add(nextCell);
                }
                else break;
            }
        }

        private bool CheckSquareMatch(GridCellHandler cell, List<GridCellHandler> neighbors)
        {
            int x = cell.GridPosition.x, y = cell.GridPosition.y;
            Vector2Int[][] offsets = {
                new [] { new Vector2Int(1,0), new Vector2Int(0,1), new Vector2Int(1,1) },
                new [] { new Vector2Int(-1,0), new Vector2Int(0,1), new Vector2Int(-1,1) },
                new [] { new Vector2Int(1,0), new Vector2Int(0,-1), new Vector2Int(1,-1) },
                new [] { new Vector2Int(-1,0), new Vector2Int(0,-1), new Vector2Int(-1,-1)}
            };

            foreach (var offSet in offsets)
            {
                List<GridCellHandler> square = new() { cell };
                if (offSet.All(off => neighbors.Any(nCell => !nCell.IsLocked && nCell.GridPosition == new Vector2Int(x + off.x, y + off.y))))
                {
                    square.AddRange(offSet.Select(off => neighbors.First(n => n.GridPosition == new Vector2Int(x + off.x, y + off.y))));
                    DestroyMatches(square);
                    return true;
                }
            }

            return false;
        }

        private bool CheckTShapeMatch(GridCellHandler cell, List<GridCellHandler> neighbors) =>
            CheckShape(cell, neighbors,
                new[] { (0, -1), (0, -2), (-1, 0), (1, 0) },
                new[] { (0, 1), (0, 2), (-1, 0), (1, 0) },
                new[] { (-1, 0), (-2, 0), (0, -1), (0, 1) },
                new[] { (1, 0), (2, 0), (0, -1), (0, 1) });

        private bool CheckLShapeMatch(GridCellHandler cell, List<GridCellHandler> neighbors) =>
            CheckShape(cell, neighbors,
                new[] { (-1, 0), (-2, 0), (0, 1), (0, 2) },
                new[] { (-1, 0), (-2, 0), (0, -1), (0, -2) },
                new[] { (1, 0), (2, 0), (0, 1), (0, 2) },
                new[] { (1, 0), (2, 0), (0, -1), (0, -2) });

        private bool CheckShape(GridCellHandler cell, List<GridCellHandler> neighbors, params (int dx, int dy)[][] shapes)
        {
            int x = cell.GridPosition.x, y = cell.GridPosition.y;

            foreach (var offsets in shapes)
            {
                if (offsets.All(o => neighbors.Any(nCell => !nCell.IsLocked && nCell.GridPosition == new Vector2Int(x + o.dx, y + o.dy))))
                {
                    List<GridCellHandler> matched = new() { cell };
                    matched.AddRange(offsets.Select(o => neighbors.First(n => n.GridPosition == new Vector2Int(x + o.dx, y + o.dy))));
                    DestroyMatches(matched);
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Neighbors

        private List<GridCellHandler> GetNeighborsOfSameType(GridCellHandler startCell)
        {
            List<GridCellHandler> connectedCells = new();

            if (startCell?.CurrentItem == null) return connectedCells;

            ItemTypes targetType = startCell.CurrentItem.ItemType;
            Queue<GridCellHandler> queue = new();
            queue.Enqueue(startCell);

            while (queue.Count > 0)
            {
                GridCellHandler cell = queue.Dequeue();
                if (cell.IsChecked || cell.CurrentItem?.ItemType != targetType) continue;

                cell.IsChecked = true;
                connectedCells.Add(cell);

                int[,] dirs = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };

                for (int i = 0; i < dirs.GetLength(0); i++)
                {
                    int nx = cell.GridPosition.x + dirs[i, 0];
                    int ny = cell.GridPosition.y + dirs[i, 1];

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

            foreach (var cell in connectedCells) cell.IsChecked = false;
            connectedCells.Remove(startCell);

            return connectedCells;
        }

        #endregion

        #region Destroy & Collapse

        // DestroyMatches
        private void DestroyMatches(List<GridCellHandler> matchedCells, bool checkForBlocks = true)
        {
            if (matchedCells == null || matchedCells.Count == 0)
                return;

            if (checkForBlocks)
            {
                List<GridCellHandler> blockingCells = new(matchedCells);

                foreach (var cell in matchedCells)
                {
                    blockingCells.AddRange(cell.CheckForBlocks(_gridCells));
                }

                matchedCells.AddRange(blockingCells);
            }

            var affectedColumns = new HashSet<int>();

            foreach (var cell in matchedCells)
            {
                if (cell == null || cell.CurrentItem == null) continue;

                if (cell.CurrentItem is MonoBehaviour mb)
                    Destroy(mb.gameObject);

                cell.CurrentItem = null;
                cell.IsChecked = false;
                affectedColumns.Add(cell.GridPosition.x);
            }

            // Her sütun için coroutine başlat
            foreach (var col in affectedColumns)
                StartCoroutine(CollapseColumnCoroutine(col));
        }

        private IEnumerator CollapseColumnCoroutine(int columnIndex)
        {
            int height = gridSize.y;

            for (int y = 0; y < height; y++)
            {
                var cell = _gridCells[columnIndex, y];

                if (cell.CurrentItem == null && !cell.IsLocked)
                {
                    // en yakın yukarıdaki dolu hücreyi bul
                    int sourceY = -1;
                    for (int yy = y + 1; yy < height; yy++)
                    {
                        if (_gridCells[columnIndex, yy].CurrentItem != null)
                        {
                            sourceY = yy;
                            break;
                        }
                    }

                    if (sourceY != -1)
                    {
                        if (_gridCells[columnIndex, sourceY].IsLocked) continue;

                        var sourceCell = _gridCells[columnIndex, sourceY];
                        var item = sourceCell.CurrentItem;
                        sourceCell.CurrentItem = null;   // kaynak hücre boşalt
                        cell.CurrentItem = item;         // hedef hücreyi DOLU işaretle
                        item.CurrentCell = cell;

                        if (item is IMovable movable)
                            movable.FallToTheCell(cell); // sadece animasyonu başlat

                    }
                    else
                    {
                        // üstte hiç yok → yeni item spawn et
                        OnSpawnNewItems(cell);
                    }

                    yield return new WaitForSeconds(0.05f); // görsel akış için küçük gecikme
                }
            }

            // Küçük bekle, sonra sütundaki her hücrede match kontrolü yap
            yield return new WaitForSeconds(0.1f);

            for (int y = 0; y < height; y++)
            {
                var c = _gridCells[columnIndex, y];
                if (c.CurrentItem != null && c.IsCheckable)
                    GridSignals.Instance.onCheckMatchesFromCell?.Invoke(c);
            }
        }
        #endregion
    }
}
