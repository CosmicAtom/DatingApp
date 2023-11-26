using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;

        public readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddMessage(Message message)
        { 
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Recipient)
                .SingleOrDefaultAsync( x=> x.Id == id);
        }

        public async Task<PageList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                .OrderByDescending(x => x.MessageSent) 
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUsername == messageParams.Username  && u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username && u.SenderDeleted == false),
                _ => query.Where(u => u.RecipientUsername == messageParams.Username && u.RecipientDeleted == false && u.DateRead == null)
            };


            return await PageList<MessageDto>.CreateAsync(query, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessagesThread(string currentUsername, string receiptUsername)
        {
            var messages = await _context.Messages
                .Where(m => m.Sender.UserName == currentUsername && m.RecipientDeleted == false && m.Recipient.UserName == receiptUsername
                      || m.Sender.UserName == receiptUsername && m.SenderDeleted == false && m.Recipient.UserName == currentUsername)
                .OrderBy(m => m.MessageSent)
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var unreadMessages = messages.Where(d => d.DateRead == null && d.RecipientUsername == currentUsername).ToList();

            if(unreadMessages.Any())
            {
                foreach (var item in unreadMessages)
                {
                    item.DateRead = DateTime.UtcNow;
                }
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public void AddGroup(Groups group)
        {
            _context.Groups.Add(group);
        }

        public void RemoveConnection(Connections connection)
        {
            _context.Connections.Remove(connection);
        }

        public async Task<Groups> GetMessageGroup(string groupName)
        {
            return await _context.Groups
                .Include(x => x.Connections)
                .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<Connections> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Groups> GetGroupsFroConnections(string ConnectionId)
        {
            return await _context.Groups
                .Include(x => x.Connections)
                .Where(c => c.Connections.Any(x => x.ConnectionsId == ConnectionId))
                .FirstOrDefaultAsync();
        }
    }
}