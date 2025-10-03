using Assets.Game.Scripts.Abstracts;
using Assets.Game.Scripts.Signals;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Game.Scripts.Handlers
{
    public class RocketHandler : AbsSpecial
    {
        public override List<GridCellHandler> GetAllAroundCells()
        {
            HashSet<GridCellHandler> destroyingCells = new HashSet<GridCellHandler>() { currentCell };

            GridCellHandler[,] gridCells = GridSignals.Instance.onGetGridCells?.Invoke();

            List<List<GridCellHandler>> detectedCells = new();

            detectedCells.Add(new List<GridCellHandler>());
            detectedCells.Add(new List<GridCellHandler>());
            detectedCells.Add(new List<GridCellHandler>());
            detectedCells.Add(new List<GridCellHandler>());

            for (int x = currentCell.GridPosition.x + 1; x < gridCells.GetLength(0); x++)
            {
                GridCellHandler cell = gridCells[x, currentCell.GridPosition.y];
                if (cell.CurrentItem != null)
                {
                    if (cell.CurrentItem is AbsBlock block)
                    {
                        if (block.CheckForImpact())
                        {
                            detectedCells[0].Add(cell);
                        }
                    }
                    else if (cell.CurrentItem is AbsSpecial special && special != this)
                    {
                        List<GridCellHandler> cells = special.GetAllAroundCells();

                        foreach (var c in cells)
                        {
                            detectedCells[0].Add(c);
                        }
                    }
                    else
                        detectedCells[0].Add(cell);
                }
            }
            for (int x = currentCell.GridPosition.x - 1; x >= 0; x--)
            {
                GridCellHandler cell = gridCells[x, currentCell.GridPosition.y];
                if (cell.CurrentItem != null)
                {
                    if (cell.CurrentItem is AbsBlock block)
                    {
                        if (block.CheckForImpact())
                        {
                            detectedCells[1].Add(cell);
                        }
                    }
                    else if (cell.CurrentItem is AbsSpecial special && special != this)
                    {
                        List<GridCellHandler> cells = special.GetAllAroundCells();

                        foreach (var c in cells)
                        {
                            detectedCells[1].Add(c);
                        }
                    }
                    else
                        detectedCells[1].Add(cell);
                }
            }
            for (int y = currentCell.GridPosition.y + 1; y < gridCells.GetLength(0); y++)
            {
                GridCellHandler cell = gridCells[currentCell.GridPosition.x, y];
                if (cell.CurrentItem != null)
                {
                    if (cell.CurrentItem is AbsBlock block)
                    {
                        if (block.CheckForImpact())
                        {
                            detectedCells[3].Add(cell);
                        }
                    }
                    else if (cell.CurrentItem is AbsSpecial special && special != this)
                    {
                        List<GridCellHandler> cells = special.GetAllAroundCells();

                        foreach (var c in cells)
                        {
                            detectedCells[3].Add(c);
                        }
                    }
                    else
                        detectedCells[3].Add(cell);
                }
            }
            for (int y = currentCell.GridPosition.y - 1; y >= 0; y--)
            {
                GridCellHandler cell = gridCells[currentCell.GridPosition.x, y];
                if (cell.CurrentItem != null)
                {
                    if (cell.CurrentItem is AbsBlock block)
                    {
                        if (block.CheckForImpact())
                        {
                            detectedCells[3].Add(cell);
                        }
                    }
                    else if (cell.CurrentItem is AbsSpecial special && special != this)
                    {
                        List<GridCellHandler> cells = special.GetAllAroundCells();

                        foreach (var c in cells)
                        {
                            detectedCells[3].Add(c);
                        }
                    }
                    else
                        detectedCells[3].Add(cell);
                }
            }

            detectedCells = detectedCells.OrderBy(x => x.Count).ToList();

            for (int i = 0; i < detectedCells[3].Count; i++)
            {
                for (int j = 0; j < detectedCells.Count; j++)
                {
                    if (detectedCells[j].Count > i)
                        destroyingCells.Add(detectedCells[j][i]);
                }
            }


            return destroyingCells.ToList();
        }
    }
}