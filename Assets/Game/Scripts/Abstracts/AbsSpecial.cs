using Assets.Game.Scripts.Signals;
using DG.Tweening;
using EditorAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Game.Scripts.Abstracts
{
    public class AbsSpecial : MonoBehaviour, IItem, IMovable
    {
        [SerializeField] private ItemTypes itemType;
        [SerializeField, ReadOnly] protected GridCellHandler currentCell;

        public GridCellHandler TargetCell { get; set; }
        public bool IsFalling { get; set; }
        public ItemTypes ItemType => itemType;
        public GridCellHandler CurrentCell { get => currentCell; set => currentCell = value; }

        public virtual List<GridCellHandler> GetAllAroundCells()
        {
            HashSet<GridCellHandler> destroyingCells = new HashSet<GridCellHandler>() { currentCell };

            GridCellHandler[,] gridCells = GridSignals.Instance.onGetGridCells?.Invoke();

            int[,] area = new int[8, 2] { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 }, { 1, 1 }, { -1, -1 }, { 1, -1 }, { -1, 1 } };

            for (int i = 0; i < area.GetLength(0); i++)
            {
                int nextX = currentCell.GridPosition.x + area[i, 0];
                int nextY = currentCell.GridPosition.y + area[i, 1];

                if (nextX < 0 || nextX >= gridCells.GetLength(0) ||
                  nextY < 0 || nextY >= gridCells.GetLength(1))
                    continue;

                GridCellHandler cell = gridCells[nextX, nextY];

                if (cell != null && cell.CurrentItem != null && !cell.IsChecked)
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

            return destroyingCells.ToList();
        }

        public void MoveToCell(GridCellHandler newCell)
        {
            currentCell = newCell;
            newCell.CurrentItem = this;

            transform.SetParent(newCell.transform);

            transform.DOLocalMove(Vector2.zero, 0.2f)
                .SetEase(Ease.InOutFlash).OnComplete(() =>
                {
                    newCell.IsCheckable = false;
                    newCell.IsLocked = false;
                    Interact();
                }).SetLink(gameObject, LinkBehaviour.KillOnDestroy);

        }

        public void Interact()
        {
            GridSignals.Instance.onDestroyMatches.Invoke(GetAllAroundCells(), false);
        }

        public void FallToTheCell(GridCellHandler targetCell)
        {
            TargetCell = targetCell;
            if (IsFalling) return;
            IsFalling = true;
            StartCoroutine(Fall(targetCell));
        }

        public IEnumerator Fall(GridCellHandler targetCell)
        {
            transform.SetParent(targetCell.transform);
            float distance = Vector2.Distance(transform.localPosition, Vector2.zero);

            float fallSpeed = 6f;

            while (distance > 0.01f)
            {
                distance = Vector2.Distance(transform.localPosition, Vector2.zero);

                if (TargetCell != targetCell)
                {
                    targetCell = TargetCell;
                    transform.SetParent(targetCell.transform);
                }

                transform.localPosition = Vector2.MoveTowards(
                    transform.localPosition,
                    Vector2.zero,
                    fallSpeed * distance * Time.deltaTime
                );
                yield return null;
            }

            transform.localPosition = Vector2.zero;

            IsFalling = false;
        }
    }
}