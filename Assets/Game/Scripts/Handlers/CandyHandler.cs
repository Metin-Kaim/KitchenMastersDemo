using Assets.Game.Scripts.Abstracts;
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
            transform.localPosition = Vector3.zero;

            print("555");
        }
    }
}