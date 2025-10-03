using Assets.Game.Scripts.Handlers;
using DG.Tweening;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Assets.Game.Scripts.Abstracts
{
    public interface IMovable
    {
        GridCellHandler TargetCell { get; set; }
        bool IsFalling { get; set; }

        void MoveToCell(GridCellHandler newCell);

        void FallToTheCell(GridCellHandler targetCell);
    }
}