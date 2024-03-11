namespace SignalRServer.Interfaces
{
    public interface ILearningHubClient
    {
        Task ReceiveMessage(string message);
    }
}
