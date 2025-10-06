
using DG.Tweening;
using Unity.Mathematics;

namespace Assets.Game.Scripts.Abstracts
{
    public interface IMovable
    {
        GridCellHandler TargetCell { get; set; }
        bool IsFalling { get; set; }
        Tweener SwapTween { get; set; }

        void MoveToCell(bool isReverse);
  
        void FallToTheCell(GridCellHandler targetCell);
    }
}