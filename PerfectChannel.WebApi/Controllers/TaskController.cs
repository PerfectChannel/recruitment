using System;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PerfectChannel.WebApi.Services;

namespace PerfectChannel.WebApi.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowOrigin")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITodoListService _todoListService;

        public TaskController(ITodoListService todoListService)
        {
            _todoListService = todoListService;
        }

        [HttpGet]
        [Route("list")]
        public IActionResult GetList()
        {
            try
            {
                var list = _todoListService.GetList();
                return Ok(list);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("add/{taskDescription}")]
        public IActionResult AddTask(string taskDescription)
        {
            try
            {
                if (!_todoListService.AddTask(taskDescription))
                {
                    return Forbid();
                }
                return Ok();
            }
            catch
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Route("changeStatus/{taskId}")]
        public IActionResult ChangeStatus(string taskId)
        {
            try
            {
                if (string.IsNullOrEmpty(taskId) || !_todoListService.ChangeStatus(taskId))
                {
                    return NotFound();
                }
                return Ok();
            }
            catch
            {
                return Forbid();
            }
        }
    }
}