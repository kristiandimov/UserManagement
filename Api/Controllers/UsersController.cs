using Common.Reposotories;
using Common.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Api.ViewModels.Users;
using Common.Dto;
using AutoMapper;
using Common.Mapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            MapperConfiguration config = new MapperConfiguration(config => config
                                                               .AddProfile(new MapperProfile()));
            IMapper mapper = config.CreateMapper();

            UserManagementDbContext context = new UserManagementDbContext();
            List<User> items = context.Users.ToList();

            return Ok(
                new
                {
                    success = true,
                    data = mapper.Map<List<UserDto>>(items)
                });
        }

        [HttpPut]
        public IActionResult Put([FromBody]CreateVM model)
        {
            MapperConfiguration config = new MapperConfiguration(config => config
                                                              .AddProfile(new MapperProfile()));
            IMapper mapper = config.CreateMapper();

            UserManagementDbContext context = new UserManagementDbContext();
            User item = new User();

            item.Username = model.Username;
            item.Password = model.Password;
            item.FirstName = model.FirstName;
            item.LastName = model.LastName;

            context.Users.Add(item);
            context.SaveChanges();

            return Ok(new
            {
                success = true,
                data = mapper.Map<UserDto>(item)
            });
        }

        [HttpPost]
        public IActionResult Post([FromBody]EditVM model)
        {

            MapperConfiguration config = new MapperConfiguration(config => config
                                                              .AddProfile(new MapperProfile()));
            IMapper mapper = config.CreateMapper();

            UserManagementDbContext context = new UserManagementDbContext();
            User item = context.Users.Where(x => x.Id == model.Id).FirstOrDefault();

            if (item == null)
            {
                return NotFound();
            }

            item.Id = model.Id;
            item.Username = model.Username;
            item.Password = model.Password;
            item.FirstName = model.FirstName;
            item.LastName = model.LastName;

            context.Users.Update(item);
            context.SaveChanges();


            return Ok(new
            {
                success = true,
                data = mapper.Map<UserDto>(item)
            });
        }

        [HttpDelete]
        public IActionResult Delete(int Id)
        {

            MapperConfiguration config = new MapperConfiguration(config => config
                                                              .AddProfile(new MapperProfile()));
            IMapper mapper = config.CreateMapper();

            UserManagementDbContext context = new UserManagementDbContext();
            User item = context.Users.Where(x => x.Id == Id).FirstOrDefault();

            if (item == null)
            {
                return NotFound();
            }
            else if (item.Projects != null)
            {
                return Problem("The User has attached Projects");
            }

            context.Users.Remove(item);
            context.SaveChanges();

            return Ok(new {
                success = true,
                data = mapper.Map<UserDto>(item)
            });

        }
    }
}

