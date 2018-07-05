namespace AutoMapper.Client.Contracts
{
    public interface ICommand
    {
        string Execute(params string[] args);
    }
}
