using Assets.Game.Scripts.Abstracts;
using Assets.Game.Scripts.Handlers;
using Assets.Game.Scripts.Signals;
using System;
using System.Collections.Generic;
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

        if (nextCell.currentItem != null && nextCell.currentItem is IMovable nextMovable)
        {
            IMovable tempCurrentMovable = currentItem as IMovable;

            nextMovable.MoveToCell(this);
            tempCurrentMovable.MoveToCell(nextCell);
        }
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
                    //else if(neighbourCell.currentItem is AbsSpecial special)
                    //{
                    //    List<GridCellHandler> impactedCells = special.CheckForImpact();
                    //    if (impactedCells != null)
                    //    {
                    //        blockingCells.AddRange(impactedCells);
                    //    }
                    //}
                }
            }
        }

        return blockingCells;
    }
}
