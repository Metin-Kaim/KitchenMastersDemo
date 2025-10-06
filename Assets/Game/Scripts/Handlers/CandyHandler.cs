using Assets.Game.Scripts.Abstracts;
using Assets.Game.Scripts.Signals;
using DG.Tweening;
using System.Collections;
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

            //transform.SetParent(targetCell.transform);
            float distance = Vector2.Distance(transform.localPosition, Vector2.zero);

            float fallSpeed = 10f;

            while (distance > 0.01f)
            {
                distance = Vector2.Distance(transform.localPosition, Vector2.zero);

                //if (TargetCell != targetCell)
                //{
                //    targetCell = TargetCell;
                //    transform.SetParent(targetCell.transform);
                //}

                transform.localPosition = Vector2.MoveTowards(
                    transform.localPosition,
                    Vector2.zero,
                    fallSpeed * distance * Time.deltaTime
                );
                yield return null;
            }

            transform.localPosition = Vector2.zero;

            //GridSignals.Instance.onCheckMatchesFromCell?.Invoke(targetCell);
            IsFalling = false;
        }
    }
}
