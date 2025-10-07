using Assets.Game.Scripts.Abstracts;
using Assets.Game.Scripts.Signals;
using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Assets.Game.Scripts.Handlers
{
    public class HybridBlockHandler : AbsBlock, IMovable
    {
        public GridCellHandler TargetCell { get; set; }
        public bool IsFalling { get; set; }
        public Tweener SwapTween { get; set; }

        public override void Init(GridCellHandler cell)
        {
            transform.localPosition = Vector3.zero;
            name += $"_({cell.GridPosition.x},{cell.GridPosition.y})";
            cell.CurrentItem = this;
            CurrentCell = cell;
            cell.IsCheckable = false;
            cell.IsLocked = false;
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
                }).SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .SetLoops(isReverse ? 2 : 0, LoopType.Yoyo);
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