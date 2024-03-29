﻿using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseAPIController
    {
        private readonly IUnitOfWork _unitOfWork;

        public LikesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await _unitOfWork.userRepository.GetUserBYUsernameAsync(username);
            var sourceUser = await _unitOfWork.likesRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null) { return NotFound(); }

            if(sourceUser.UserName  == username) { return BadRequest("You cannot liked yourself"); }

            var userLike = await _unitOfWork.likesRepository.GetUserLikes(sourceUserId, likedUser.Id);
            
            if (userLike != null) { return BadRequest("You already like this user"); }

            userLike = new UserLike { SourceUserId = sourceUserId, LikedUserId = likedUser.Id };

            sourceUser.LikedUser.Add(userLike);

            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Filed to like user");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikesDto>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var  users = await _unitOfWork.likesRepository.GetUserLikes(likesParams);

            Response.AddPaginationheader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(users);
        }
    }
}
