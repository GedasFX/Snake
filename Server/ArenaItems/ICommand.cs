namespace Server.ArenaItems
{
    public interface ICommand
    {
        void Interact(Snek snek);
        void Undo(Snek snek);
    }
}