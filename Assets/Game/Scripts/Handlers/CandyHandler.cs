using Assets.Game.Scripts.Abstracts;
using Assets.Game.Scripts.Signals;
using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Assets.Game.Scripts.Handlers
{
    public class CandyHandler : MonoBehaviour, IItem, IMovable
    {
        #region Fields

        [SerializeField] private ItemTypes _itemType;
        [SerializeField] private GridCellHandler _currentCell;

        private bool _isFalling = false;
        private GridCellHandler targetCell;

        #endregion

        #region Properties

        public ItemTypes ItemType => _itemType;

        public GridCellHandler CurrentCell
        {
            get => _currentCell;
            set => _currentCell = value;
        }

        #endregion

        #region Public Methods

        public void MoveToCell(GridCellHandler newCell)
        {
            _currentCell = newCell;
            newCell.CurrentItem = this;

            transform.SetParent(newCell.transform);

            transform.DOLocalMove(Vector2.zero, 0.2f)
                .SetEase(Ease.InOutFlash)
                .OnComplete(() =>
                {
                    newCell.IsBlocked = false;
                    GridSignals.Instance.onCheckMatchesFromCell?.Invoke(newCell);
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

            // ❌ CurrentItem setleme yok, GridManager zaten yaptı

            GridSignals.Instance.onCheckMatchesFromCell?.Invoke(targetCell);
            _isFalling = false;
        }

        #endregion
    }
}
