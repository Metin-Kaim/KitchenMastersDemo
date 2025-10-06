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
            if (currentCell.IsChecked) return new List<GridCellHandler>();

            HashSet<GridCellHandler> destroyingCells = new HashSet<GridCellHandler>() { currentCell };
            currentCell.IsChecked = true;

            GridCellHandler[,] gridCells = GridSignals.Instance.onGetGridCells?.Invoke();

            for (int i = 0; i < gridCells.GetLength(0); i++)
            {
                GridCellHandler cell = gridCells[i, currentCell.GridPosition.y];

                if (cell == currentCell || cell.IsChecked) continue;

                CheckTheCellByItemType(destroyingCells, cell);
            }
            for (int i = 0; i < gridCells.GetLength(1); i++)
            {
                GridCellHandler cell = gridCells[currentCell.GridPosition.x, i];

                if (cell == currentCell || cell.IsChecked) continue;

                CheckTheCellByItemType(destroyingCells, cell);
            }

            return destroyingCells.ToList();
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
                        c.IsChecked = true;
                        destroyingCells.Add(c);
                    }
                }
                else if (cell.CurrentItem != null)
                {
                    cell.IsChecked = true;
                    destroyingCells.Add(cell);
                }
            }
        }
    }
}