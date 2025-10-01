using Assets.Game.Scripts.Handlers;
using UnityEngine;

namespace Assets.Game.Scripts.Abstracts
{
    public interface IMovable
    {
        void MoveToCell(GridCellHandler newCell);
    }
}