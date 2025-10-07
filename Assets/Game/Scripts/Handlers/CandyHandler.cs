using Assets.Game.Scripts.Abstracts;
using Assets.Game.Scripts.Signals;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Game.Scripts.Handlers
{
    public class CandyHandler : MonoBehaviour, IItem, IMovable
    {
        [SerializeField] private ItemTypes _itemType;
        [SerializeField] private GridCellHandler currentCell;

        public Tweener SwapTween { get; set; }
        public ItemTypes ItemType => _itemType;
        public GridCellHandler CurrentCell
        {
            get => currentCell;
            set => currentCell = value;
        }
        public GridCellHandler TargetCell { get; set; }
        public bool IsFalling { get; set; }

        public void MoveToCell(bool isReverse)
        {
            transform.SetParent(currentCell.transform);

            SwapTween?.Complete();

            SwapTween = transform.DOLocalMove(Vector2.zero, 0.2f)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    currentCell.IsCheckable = true;
                    currentCell.IsLocked = false;
                    SwapTween = null;
                    transform.SetParent(currentCell.transform);
                    //GridSignals.Instance.onCheckMatchesFromCell?.Invoke(newCell);
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
