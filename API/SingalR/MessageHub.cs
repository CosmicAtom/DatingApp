using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SingalR
{
    public class MessageHub : Hub                           
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHubContext<PresenceHub> _presenceContext;
        private readonly PresenceTracker _presenceTracker;

        public MessageHub(IUnitOfWork unitOfWork, IMapper mapper, 
            IHubContext<PresenceHub> presenceContext, PresenceTracker presenceTracker)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _presenceContext = presenceContext;
            _presenceTracker = presenceTracker;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"].ToString();

            var groupName = GetGroupName(Context.User.GetUsername(),otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var group = await AddToGroup(groupName);
            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

            var messages = await _unitOfWork.messageRepository.GetMessagesThread(Context.User.GetUsername(), otherUser);

            if (_unitOfWork.HasChange()) await _unitOfWork.Complete();

            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username = Context.User.GetUsername();

            if (username == createMessageDto.RecipientUsername.ToLower()) 
                    { throw new HubException("You can send message to yourself"); }

            var sender = await _unitOfWork.userRepository.GetUserBYUsernameAsync(username);
            var recepient = await _unitOfWork.userRepository.GetUserBYUsernameAsync(createMessageDto.RecipientUsername) ?? throw new HubException("User not found");
            
            var message = new Message
            {
                Sender = sender,
                Recipient = recepient,
                SenderUsername = sender.UserName,
                RecipientUsername = recepient.UserName,
                Content = createMessageDto.Content
            };

            var groupName = GetGroupName(sender.UserName, recepient.UserName);

            var group = await _unitOfWork.messageRepository.GetMessageGroup(groupName);

            if(group.Connections.Any(x => x.Username == recepient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else
            {
                var connections = await _presenceTracker.GetCConnnectionsForUser(recepient.UserName);
                if(connections != null)
                {
                    await _presenceContext.Clients.Clients(connections).SendAsync("NewMessageReceived"
                        , new
                        {
                            username = sender.UserName,
                            knownAs = sender.KnownAs
                        });
                }
            }

            _unitOfWork.messageRepository.AddMessage(message);

            if (await _unitOfWork.Complete())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message)); 
            }
        }

        #region PRIVATE METHODS
        private static string GetGroupName(string caller, string other)
        {
            var strCompareResult = string.CompareOrdinal(caller, other) < 0;
            return strCompareResult ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        private async Task<Groups> AddToGroup(string groupName)
        {
            var group = await _unitOfWork.messageRepository.GetMessageGroup(groupName);
            var connection = new Connections(Context.ConnectionId, Context.User.GetUsername());

            if (group == null)
            {
                group = new Groups(groupName);
                _unitOfWork.messageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);
            if(await _unitOfWork.Complete()) return group;

            throw new HubException("Filed to join Group.");
        }

        private async Task<Groups> RemoveFromMessageGroup()
        {
            var group = await _unitOfWork.messageRepository.GetGroupsFroConnections(Context.ConnectionId);
            var connection = await _unitOfWork.messageRepository.GetConnection(Context.ConnectionId);
            _unitOfWork.messageRepository.RemoveConnection(connection);
            if(await _unitOfWork.Complete()) return group;
            throw new HubException("Filed to remove from Group.");
        }
        #endregion
    }
}
