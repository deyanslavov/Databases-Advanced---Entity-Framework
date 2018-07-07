namespace PhotoShare.Client.Contracts
{
    public interface IExecutable
    {
        string Execute(params string[] args);
    }
}
