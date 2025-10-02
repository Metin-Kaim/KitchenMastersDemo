using Assets.Game.Scripts.Handlers;
using UnityEngine;

namespace Assets.Game.Scripts.Abstracts
{
    public interface IItem
    {
        ItemTypes ItemType { get; }
        GridCellHandler CurrentCell { get; set; }

        //Vector2 Position { get; set; }
        //void Use();
        //void Discard();
    }
}