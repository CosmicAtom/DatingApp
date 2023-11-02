using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseAPIController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        public readonly IMapper _mapper;

        public MessagesController(IUserRepository userRepositor, IMessageRepository messageRepository, IMapper mapper)
        {
            _userRepository = userRepositor;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername();

            if(username == createMessageDto.RecipientUsername.ToLower()) { return BadRequest("You can set message to yourself"); }

            var sender = await _userRepository.GetUserBYUsernameAsync(username);
            var recepient = await _userRepository.GetUserBYUsernameAsync(createMessageDto.RecipientUsername);

            if (recepient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recepient,
                SenderUsername = sender.UserName,
                RecipientUsername = recepient.UserName,
                Content = createMessageDto.Content
            };

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Field to send message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();

            var messages = await _messageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationheader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

            return messages;
        }
    }
}