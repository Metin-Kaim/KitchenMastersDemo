using EditorAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.Abstracts
{
    public abstract class AbsBlock : MonoBehaviour, IItem
    {
        [SerializeField] private ItemTypes itemType;
        [SerializeField] private List<Sprite> blockSprites;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private int impactResistance = 1;

        [SerializeField, ReadOnly] protected GridCellHandler currentCell;

        public ItemTypes ItemType => itemType;
        public GridCellHandler CurrentCell { get => currentCell; set => currentCell = value; }

        internal bool CheckForImpact()
        {
            impactResistance--;

            if (impactResistance <= 0)
            {
                currentCell.IsBlocked = false;
                currentCell.IsLocked = false;
                return true;
            }
            else
            {
                if (blockSprites.Count > 0)
                    spriteRenderer.sprite = blockSprites[impactResistance - 1];
                return false;
            }
        }
    }
}