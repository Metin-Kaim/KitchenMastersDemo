using Assets.Game.Scripts.Abstracts;
using Assets.Game.Scripts.Signals;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridCellHandler : MonoBehaviour
{
    [SerializeField] private IItem currentItem;
    [SerializeField] private Vector2Int gridPosition;
    [SerializeField] private bool isCheckable;
    [SerializeField] private bool isLocked;

    public IItem CurrentItem { get => currentItem; set => currentItem = value; }
    public Vector2Int GridPosition { get => gridPosition; set => gridPosition = value; }
    public bool IsCheckable { get => isCheckable; set => isCheckable = value; }
    public bool IsChecked { get; internal set; }
    public bool IsLocked { get => isLocked; set => isLocked = value; }

    public void SwapItemWithNeighbourCell(Vector2Int direction)
    {
        if (currentItem == null) return;

        GridCellHandler[,] cells = GridSignals.Instance.onGetGridCells?.Invoke();
        if (cells == null) return;

        Vector2Int gridSize = GridSignals.Instance.onGetGridSize.Invoke();
        Vector2Int neighbourPos = gridPosition + direction;

        if (neighbourPos.x >= gridSize.x || neighbourPos.x < 0 || neighbourPos.y >= gridSize.y || neighbourPos.y < 0)
            return;

        GridCellHandler nextCell = cells[neighbourPos.x, neighbourPos.y];

        if (nextCell.currentItem != null)
        {
            if (currentItem is AbsSpecial _ && nextCell.currentItem is AbsSpecial _)
            {
                BeginTheCombonation(this, nextCell);
            }
            else if (nextCell.currentItem is IMovable nextMovable)
            {
                IMovable tempCurrentMovable = currentItem as IMovable;

                IItem tempItem = currentItem;
                currentItem = nextCell.currentItem;
                nextCell.currentItem = tempItem;
                nextCell.currentItem.CurrentCell = nextCell;
                currentItem.CurrentCell = this;

                List<GridCellHandler> matches1 = GridSignals.Instance.onCheckMatchesFromCell?.Invoke(this);
                List<GridCellHandler> matches2 = GridSignals.Instance.onCheckMatchesFromCell?.Invoke(nextCell);

                if (matches1.Count > 0 || matches2.Count > 0 || nextCell.currentItem is AbsSpecial _ || currentItem is AbsSpecial _)
                {
                    matches1.AddRange(matches2);

                    nextMovable.MoveToCell(false);
                    tempCurrentMovable.MoveToCell(false);

                    if (matches1.Count > 0)
                        StartCoroutine(SwapItems(matches1));
                }
                else
                {
                    nextMovable.MoveToCell(true);
                    tempCurrentMovable.MoveToCell(true);

                    nextCell.currentItem = currentItem;
                    currentItem = tempItem;
                    nextCell.currentItem.CurrentCell = nextCell;
                    currentItem.CurrentCell = this;
                }

            }
        }
    }
    public IEnumerator SwapItems(List<GridCellHandler> cells)
    {
        yield return new WaitForSeconds(.2f);

        GridSignals.Instance.onDestroyMatches?.Invoke(cells, true);
    }

    public List<GridCellHandler> CheckForBlocks(GridCellHandler[,] gridCells)
    {
        int[,] directions = new int[4, 2] { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 } };

        List<GridCellHandler> blockingCells = new List<GridCellHandler>();

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            Vector2Int neighbourPos = gridPosition + new Vector2Int(directions[i, 0], directions[i, 1]);
            if (neighbourPos.x >= 0 && neighbourPos.x < gridCells.GetLength(0) &&
                neighbourPos.y >= 0 && neighbourPos.y < gridCells.GetLength(1))
            {
                GridCellHandler neighbourCell = gridCells[neighbourPos.x, neighbourPos.y];
                if (neighbourCell.currentItem != null && !neighbourCell.isCheckable)
                {
                    if ((neighbourCell.currentItem is AbsBlock block) && block.CheckForImpact())
                    {
                        blockingCells.Add(neighbourCell);
                    }
                }
            }
        }

        return blockingCells;
    }

    private void BeginTheCombonation(GridCellHandler cell1, GridCellHandler cell2)
    {
        MonoBehaviour cell1Item = (cell1.currentItem as MonoBehaviour);
        cell1Item.transform.SetParent(cell2.transform);

        cell1Item.transform.DOLocalMove(Vector2.zero, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            ApplyCombo(cell1, cell2);
        });
    }

    private void ApplyCombo(GridCellHandler cell1, GridCellHandler cell2)
    {
        GridCellHandler[,] gridCells = GridSignals.Instance.onGetGridCells?.Invoke();

        HashSet<GridCellHandler> destroyingCells = new HashSet<GridCellHandler>() { cell1, cell2 };

        ItemTypes itemType1 = cell1.currentItem.ItemType;
        ItemTypes itemType2 = cell2.currentItem.ItemType;

        cell1.IsChecked = true;
        cell2.IsChecked = true;

        if (itemType1 == ItemTypes.Bomb && itemType2 == ItemTypes.Bomb)
        {
            int[,] area = new int[,] { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 }, { 1, 1 }, { -1, -1 }, { 1, -1 }, { -1, 1 }, { 2, 0 }, { 2, 1 }, { 2, 2 }, { 1, 2 }, { 0, 2 }, { -1, 2 }, { -2, 2 }, { -2, -1 }, { -2, 0 }, { -2, -1 }, { -2, -2 }, { -1, -2 }, { -0, -2 }, { 1, -2 }, { 2, -2 }, { 2, -1 } };

            for (int i = 0; i < area.GetLength(0); i++)
            {
                int nextX = cell2.gridPosition.x + area[i, 0];
                int nextY = cell2.gridPosition.y + area[i, 1];

                if (nextX < 0 || nextX >= gridCells.GetLength(0) ||
                  nextY < 0 || nextY >= gridCells.GetLength(1))
                    continue;

                GridCellHandler cell = gridCells[nextX, nextY];

                if (cell == cell1 || cell == cell2) continue;
                if (cell.IsChecked) continue;

                CheckTheCellByItemType(destroyingCells, cell);
            }
        }
        else if ((itemType1 == ItemTypes.Bomb && itemType2 == ItemTypes.Rocket) || (itemType1 == ItemTypes.Rocket && itemType2 == ItemTypes.Bomb))
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = 0; j < gridCells.GetLength(0); j++)
                {
                    GridCellHandler cell = gridCells[j, cell2.gridPosition.y + i];

                    if (cell == cell1 || cell == cell2) continue;
                    if (cell.IsChecked) continue;
                    CheckTheCellByItemType(destroyingCells, cell);
                }
                for (int j = 0; j < gridCells.GetLength(1); j++)
                {
                    GridCellHandler cell = gridCells[cell2.gridPosition.x + i, j];

                    if (cell == cell1 || cell == cell2) continue;
                    if (cell.IsChecked) continue;
                    CheckTheCellByItemType(destroyingCells, cell);
                }
            }
        }
        else if (itemType1 == ItemTypes.Rocket && itemType2 == ItemTypes.Rocket)
        {
            for (int i = 0; i < gridCells.GetLength(0); i++)
            {
                GridCellHandler cell = gridCells[i, cell2.gridPosition.y];

                if (cell == cell1 || cell == cell2) continue;
                if (cell.IsChecked) continue;
                CheckTheCellByItemType(destroyingCells, cell);
            }
            for (int i = 0; i < gridCells.GetLength(1); i++)
            {
                GridCellHandler cell = gridCells[cell2.gridPosition.x, i];

                if (cell == cell1 || cell == cell2) continue;
                if (cell.IsChecked) continue;
                CheckTheCellByItemType(destroyingCells, cell);
            }

            Vector2Int[] diagonalDirs = new Vector2Int[]
            {
                new Vector2Int(-1, -1),
                new Vector2Int(-1, +1),
                new Vector2Int(+1, -1),
                new Vector2Int(+1, +1)
            };

            foreach (var dir in diagonalDirs)
            {
                int checkX = cell2.gridPosition.x + dir.x;
                int checkY = cell2.gridPosition.y + dir.y;

                while (checkX >= 0 && checkY >= 0 &&
                       checkX < gridCells.GetLength(0) &&
                       checkY < gridCells.GetLength(1))
                {
                    GridCellHandler cell = gridCells[checkX, checkY];

                    if (cell == cell1 || cell == cell2 || cell.IsChecked)
                    {
                        checkX += dir.x;
                        checkY += dir.y;
                        continue;
                    }

                    CheckTheCellByItemType(destroyingCells, cell);

                    checkX += dir.x;
                    checkY += dir.y;
                }
            }

        }

        if (destroyingCells.Count > 0)
        {
            foreach (GridCellHandler cell in destroyingCells)
                cell.IsChecked = false;

            GridSignals.Instance.onDestroyMatches?.Invoke(destroyingCells.ToList(), false);
        }
    }

    private void CheckTheCellByItemType(HashSet<GridCellHandler> destroyingCells, GridCellHandler cell)
    {
        if (cell != null && cell.CurrentItem != null)
        {
            if (cell.CurrentItem is AbsBlock block)
            {
                if (block.CheckForImpact())
                {
                    cell.IsChecked = true;
                    destroyingCells.Add(cell);
                }
            }
            else if (cell.CurrentItem is AbsSpecial special && special != this)
            {
                List<GridCellHandler> cells = special.GetAllAroundCells();

                foreach (var c in cells)
                {
                    cell.IsChecked = true;
                    destroyingCells.Add(c);
                }
            }
            else
            {
                cell.IsChecked = true;
                destroyingCells.Add(cell);
            }

        }
    }
}
