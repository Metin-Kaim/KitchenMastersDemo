using Assets.Game.Scripts.Abstracts;
using UnityEngine;

namespace Assets.Game.Scripts.Handlers
{
    public class GridCellHandler : MonoBehaviour
    {
        [SerializeField] private IItem currentItem;

        public IItem CurrentItem { get => currentItem; set => currentItem = value; }
    }
}