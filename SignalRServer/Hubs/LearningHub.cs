using Microsoft.AspNetCore.SignalR;
using SignalRServer.Interfaces;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace SignalRServer.Hubs
{
    public class LearningHub : Hub<ILearningHubClient>
    {


        public string GetMessageToSend(string originalMessage)
        {
            return $"User connection id: {Context.ConnectionId}. Message: {originalMessage}";
        }

        public async Task BroadcastMessage(string message)
        {
            await Clients.All.ReceiveMessage(GetMessageToSend(message));
        }

        public async Task SendToOthers(string message)
        {
            await Clients.Others.ReceiveMessage(GetMessageToSend(message));
        }

        public async Task SendToCaller(string message)
        {
            await Clients.Caller.ReceiveMessage(GetMessageToSend(message));
        }

        public async Task SendToIndividual(string connectionId, string message)
        {
            await Clients.Client(connectionId).ReceiveMessage(GetMessageToSend(message));
        }
        public async Task SendToMultipleIndividuals(List<string> connectionIds, string message)
        {
            await Clients.Clients(connectionIds).ReceiveMessage(GetMessageToSend(message));
        }

        #region Group
        public async Task SendToGroup(string groupName, string message)
        {
            await Clients.Group(groupName).ReceiveMessage(GetMessageToSend(message));
        }

        public async Task AddUserToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Caller.ReceiveMessage($"Current user added to {groupName} group");
            await Clients.Others.ReceiveMessage($"User {Context.ConnectionId} added to {groupName} group");
        }

        public async Task RemoveUserFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Caller.ReceiveMessage($"Current user removed from {groupName} group");
            await Clients.Others.ReceiveMessage($"User {Context.ConnectionId} removed from {groupName} group");
        }

        #endregion

        #region Streaming
        public async Task BroadcastStream(IAsyncEnumerable<string> stream)
        {
            await foreach (var streamItem in stream)
            {
                await Clients.Caller.ReceiveMessage($"Server received {streamItem}");

            }
        }

        public async IAsyncEnumerable<string> TriggerStream(int jobsCount, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            for (int i = 0; i < jobsCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return $"Job {i} executed successfully";
                await Task.Delay(1000, cancellationToken);
            }
        }

        private async Task WriteItemsAsync(ChannelWriter<int> writer, int jobsCount, CancellationToken cancellationToken)
        {
            Exception localException = null;
            try
            {
                for (var i = 0; i < jobsCount; i++)
                {
                    await writer.WriteAsync(i, cancellationToken);
                    await Task.Delay(1000, cancellationToken);
                }
            }
            catch (Exception ex)
            {

                localException = ex;
            }
            finally
            {
                writer.Complete(localException);
            }
        }

        #endregion

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
