using Api.ViewModels.Tasks;
using AutoMapper;
using Common.Dto;
using Common.Entities;
using Common.Mapper;
using Common.Reposotories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            MapperConfiguration config = new MapperConfiguration(config => config
                                                                .AddProfile(new MapperProfile()));
            IMapper mapper = config.CreateMapper();

            UserManagementDbContext context = new UserManagementDbContext();
            List<Task> items = context.Tasks.ToList();

            return Ok(new
            {
                success = true,
                data = mapper.Map<List<TaskDto>>(items)
            });
        }

        [HttpPut]
        public IActionResult Put([FromBody] CreateVM model)
        {
            MapperConfiguration config = new MapperConfiguration(config => config
                                                                .AddProfile(new MapperProfile()));
            IMapper mapper = config.CreateMapper();

            UserManagementDbContext context = new UserManagementDbContext();
            Task item = new Task();

            item.Title = model.Title;
            item.Description = model.Description;
            item.StoryPoints = model.StoryPoints;
            item.CreatedTime = System.DateTime.Now;
            item.UpdatedTime = System.DateTime.Now;
            item.ProjectId = model.ProjectId;
            item.UserId = model.UserId;

            //Validation if the UserId is valid and ProjectId and the User and the Project need to be linked together

            context.Tasks.Add(item);
            context.SaveChanges();

            return Ok(new
            {
                success = true,
                data = mapper.Map<TaskDto>(item)
            });
        }

        [HttpPost]
        public IActionResult Post([FromBody] EditVM model)
        {
            MapperConfiguration config = new MapperConfiguration(config => config
                                                                .AddProfile(new MapperProfile()));
            IMapper mapper = config.CreateMapper();

            UserManagementDbContext context = new UserManagementDbContext();
            Task item = context.Tasks.Where(x => x.Id == model.Id).FirstOrDefault();

            if (item == null)
            {
                return NotFound();
            }

            item.Title = model.Title;
            item.Description = model.Description;
            item.StoryPoints = model.StoryPoints;
            //item.CreatedTime = model.CreatedTime;
            item.UpdatedTime = System.DateTime.Now;
            item.ProjectId = model.ProjectId;
            item.UserId = model.UserId;

            //Validation if the UserId is valid and ProjectId and the User and the Project need to be linked together

            context.Tasks.Update(item);
            context.SaveChanges();

            return Ok(new
            {
                success = true,
                data = mapper.Map<TaskDto>(item)
            });
        }

        [HttpDelete]
        public IActionResult Delete(int Id)
        {
            MapperConfiguration config = new MapperConfiguration(config => config
                                                                .AddProfile(new MapperProfile()));
            IMapper mapper = config.CreateMapper();

            UserManagementDbContext context = new UserManagementDbContext();
            Task item = context.Tasks.Where(x => x.Id == Id).FirstOrDefault();

            if (item == null)
            {
                return NotFound();
            }

            context.Tasks.Remove(item);
            context.SaveChanges();

            return Ok(new
            {
                success = true,
                data = mapper.Map<TaskDto>(item)
            });
        }
    }

}
