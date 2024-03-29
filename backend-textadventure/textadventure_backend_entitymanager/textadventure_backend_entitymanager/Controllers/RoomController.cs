﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using textadventure_backend_entitymanager.Helpers;
using textadventure_backend_entitymanager.Models.Requests;
using textadventure_backend_entitymanager.Services;

namespace textadventure_backend_entitymanager.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService roomService;
        private readonly AccessTokenHelper accessTokenHelper;
        public RoomController(IRoomService _roomService, AccessTokenHelper _accessTokenHelper)
        {
            roomService = _roomService;
            accessTokenHelper = _accessTokenHelper;
        }

        [AllowAnonymous]
        [HttpGet("get/{roomId}/{accessToken}")]
        public async Task<IActionResult> GetRoom([FromRoute] int roomId, string accesstoken)
        {
            if (!accessTokenHelper.IsTokenValid(accesstoken))
            {
                return Unauthorized("Invalid accesstoken");
            }

            try
            {
                var response = await roomService.Find(roomId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("spawn/{adventurerId}/{accessToken}")]
        public async Task<IActionResult> CreateSpawn([FromRoute] string accesstoken, int adventurerId)
        {
            if (!accessTokenHelper.IsTokenValid(accesstoken))
            {
                return Unauthorized("Invalid accesstoken");
            }

            try
            {
                await roomService.CreateSpawn(adventurerId);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("complete/{adventurerId}/{accessToken}")]
        public async Task<IActionResult> CompleteRoom([FromRoute] string accesstoken, int adventurerId)
        {
            if (!accessTokenHelper.IsTokenValid(accesstoken))
            {
                return Unauthorized("Invalid accesstoken");
            }

            try
            {
                await roomService.CompleteRoom(adventurerId);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("move-to/{accessToken}")]
        public async Task<IActionResult> EnterRoom([FromRoute] string accesstoken, [FromBody] EnterRoomRequest enterRoomRequest)
        {
            if (!accessTokenHelper.IsTokenValid(accesstoken))
            {
                return Unauthorized("Invalid accesstoken");
            }

            try
            {
                var result = await roomService.MoveToRoom(enterRoomRequest.AdventurerId, enterRoomRequest.Direction.ToLower());

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
