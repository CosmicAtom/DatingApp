using System.Text.RegularExpressions;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IMessageRepository
    {
        void AddGroup(Groups group);
        void RemoveConnection(Connections connection);
        Task<Groups> GetMessageGroup(string groupName);
        Task<Connections> GetConnection(string ConnectionId);
        Task<Groups> GetGroupsFroConnections(string ConnectionId);
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(int id);
        Task<PageList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessagesThread(string currentUsername, string receiptUsername);
    }
}