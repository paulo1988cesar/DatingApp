using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Data.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
       private readonly IDatingRepository _repo;
       private readonly IMapper _mapper;

       public UsersController(IDatingRepository repo, IMapper mapper)
       {
         _repo = repo;  
         _mapper = mapper;
       } 

       [HttpGet]
       public async Task<IActionResult> GetUsers()
       {
           var user = await _repo.GetUsers();

           var userToReturn = _mapper.Map<IEnumerable<UserForListDto>>(user);

           return Ok(userToReturn);
       }

       [HttpGet("{id}")]
       public async Task<IActionResult> GetUser(int id)
       {
           var user = await _repo.GetUser(id);

           var userToReturn = _mapper.Map<UserForDetailedDto>(user); 

           return Ok(userToReturn);
       }
    }
}