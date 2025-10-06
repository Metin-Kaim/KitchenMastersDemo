
namespace Assets.Game.Scripts.Abstracts
{
    public interface IItem
    {
        ItemTypes ItemType { get; }
        GridCellHandler CurrentCell { get; set; }
    }
}