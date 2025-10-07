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

        public Tweener SwapTween { get; set; }
        public GridCellHandler TargetCell { get; set; }
        public bool IsFalling { get; set; }
        public ItemTypes ItemType => itemType;
        public GridCellHandler CurrentCell { get => currentCell; set => currentCell = value; }

        public void Init(GridCellHandler cell)
        {
            transform.localPosition = Vector3.zero;
            name += $"_({cell.GridPosition.x},{cell.GridPosition.y})";
            cell.CurrentItem = this;
            CurrentCell = cell;
            cell.IsCheckable = false;
            cell.IsLocked = false;
        }

        public virtual List<GridCellHandler> GetAllAroundCells()
        {
            HashSet<GridCellHandler> destroyingCells = new HashSet<GridCellHandler>() { currentCell };

            currentCell.IsChecked = true;

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

        public void MoveToCell(bool isReverse)
        {
            transform.SetParent(currentCell.transform);

            SwapTween?.Complete();

            SwapTween = transform.DOLocalMove(Vector2.zero, 0.2f)
                .SetEase(Ease.Linear).OnComplete(() =>
                {
                    currentCell.IsCheckable = false;
                    currentCell.IsLocked = false;
                    SwapTween = null;
                    transform.SetParent(currentCell.transform);
                    Interact();
                }).SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                 .SetLoops(isReverse ? 2 : 0, LoopType.Yoyo);

        }

        public void Interact()
        {
            GridSignals.Instance.onDestroyMatches.Invoke(GetAllAroundCells(), false);
        }

        public void FallToTheCell(GridCellHandler targetCell)
        {
            TargetCell = targetCell;
            transform.SetParent(targetCell.transform);
            if (IsFalling) return;
            IsFalling = true;
            StartCoroutine(Fall());
        }

        public IEnumerator Fall()
        {
            SwapTween?.Complete();

            GridCellHandler[,] gridCells = GridSignals.Instance.onGetGridCells.Invoke();

            //transform.SetParent(targetCell.transform);
            float distance = Vector2.Distance(transform.localPosition, Vector2.zero);

            float fallSpeed = 10f;

            while (distance > 0.01f)
            {
                distance = Vector2.Distance(transform.localPosition, Vector2.zero);

                transform.localPosition = Vector2.MoveTowards(
                    transform.localPosition,
                    Vector2.zero,
                    fallSpeed * distance * Time.deltaTime
                );

                if (distance < 0.1f)
                {
                    for (int i = -1; i < 2; i += 2)
                    {
                        if (currentCell.GridPosition.x + i >= gridCells.GetLength(0) || currentCell.GridPosition.x + i < 0 || currentCell.GridPosition.y - 1 < 0) continue;

                        GridCellHandler crossCell = gridCells[currentCell.GridPosition.x + i, currentCell.GridPosition.y - 1];
                        if (crossCell.CurrentItem == null)
                        {
                            GridCellHandler oldCell = currentCell;

                            crossCell.CurrentItem = this;
                            currentCell.CurrentItem = null;
                            currentCell = crossCell;
                            transform.SetParent(crossCell.transform);
                            GridSignals.Instance.onManuelCollapseColumn?.Invoke(currentCell.GridPosition.x);
                            GridSignals.Instance.onManuelCollapseColumn?.Invoke(oldCell.GridPosition.x);
                            break;
                        }
                    }
                }

                yield return null;
            }

            transform.localPosition = Vector2.zero;

            IsFalling = false;
        }
    }
}