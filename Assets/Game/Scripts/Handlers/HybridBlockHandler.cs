using Assets.Game.Scripts.Abstracts;
using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Assets.Game.Scripts.Handlers
{
    public class HybridBlockHandler : AbsBlock, IMovable
    {
        public GridCellHandler TargetCell { get; set; }
        public bool IsFalling { get; set; }

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
                }).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
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