using Assets.Game.Scripts.Abstracts;
using Assets.Game.Scripts.Signals;
using UnityEngine;

namespace Assets.Game.Scripts.Handlers
{
    public class GridCellHandler : MonoBehaviour
    {
        [SerializeField] private IItem currentItem;
        [SerializeField] private Vector2Int gridPosition;
        [SerializeField] private bool isBlocked;

        public IItem CurrentItem { get => currentItem; set => currentItem = value; }
        public Vector2Int GridPosition { get => gridPosition; set => gridPosition = value; }
        public bool IsBlocked { get => isBlocked; set => isBlocked = value; }

        public void SwapItemWithNeighbourCell(Vector2Int direction)
        {
            if (currentItem == null) return;

            GridCellHandler[,] cells = GridSignals.Instance.onGetGridCells?.Invoke();

            if (cells == null) return;

            Vector2Int gridSize = GridSignals.Instance.onGetGridSize.Invoke();
            Vector2Int neighbourPos = gridPosition + direction;

            if (neighbourPos.x >= gridSize.x || neighbourPos.x < 0 || neighbourPos.y >= gridSize.y || neighbourPos.y < 0)
            {
                Debug.Log("Neighbour cell is out of bounds.");
                return;
            }

            GridCellHandler nextCell = cells[gridPosition.x + direction.x, gridPosition.y + direction.y];

            if (nextCell.currentItem != null && nextCell.currentItem is IMovable nextMovable)
            {
                IMovable tempCurrentMovable = currentItem as IMovable;

                isBlocked = true;
                nextCell.isBlocked = true;

                nextMovable.MoveToCell(this);
                tempCurrentMovable.MoveToCell(nextCell);
            }
        }
    }
}