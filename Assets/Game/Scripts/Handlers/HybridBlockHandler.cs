using Assets.Game.Scripts.Abstracts;
using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Assets.Game.Scripts.Handlers
{
    public class HybridBlockHandler : AbsBlock, IMovable
    {
        private bool _isFalling = false;
        private GridCellHandler targetCell;

        public void MoveToCell(GridCellHandler newCell)
        {
            currentCell = newCell;
            newCell.CurrentItem = this;

            transform.SetParent(newCell.transform);

            transform.DOLocalMove(Vector2.zero, 0.2f)
                .SetEase(Ease.InOutFlash).OnComplete(() =>
                {
                    newCell.IsBlocked = true;
                    newCell.IsLocked = false;
                });
        }

        public void FallToTheCell(GridCellHandler targetCell)
        {
            this.targetCell = targetCell;
            if (_isFalling) return;
            _isFalling = true;
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

                if (this.targetCell != targetCell)
                {
                    targetCell = this.targetCell;
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

            _isFalling = false;
        }
    }
}