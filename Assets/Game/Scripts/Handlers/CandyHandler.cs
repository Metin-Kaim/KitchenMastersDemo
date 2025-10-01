using Assets.Game.Scripts.Abstracts;
using DG.Tweening;
using UnityEngine;

namespace Assets.Game.Scripts.Handlers
{
    public class CandyHandler : MonoBehaviour, IItem, IMovable
    {
        [SerializeField] private GridCellHandler _currentCell;

        public void MoveToCell(GridCellHandler newCell)
        {
            _currentCell = newCell;
            newCell.CurrentItem = this;
            transform.SetParent(newCell.transform);
            transform.DOLocalMove(Vector2.zero, 0.2f).SetEase(Ease.InOutFlash).OnComplete(() => newCell.IsBlocked = false);
        }
    }
}