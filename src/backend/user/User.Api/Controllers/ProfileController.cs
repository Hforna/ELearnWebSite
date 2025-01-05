using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using User.Api.Attributes;
using User.Api.DTOs;
using User.Api.Filters.Token;
using User.Api.Models;
using User.Api.Models.Repositories;
using User.Api.Responses;
using User.Api.Services;
using User.Api.Services.Security.Token;

namespace User.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly ITokenReceptor tokenReceptor;
        private readonly ImageService _imageService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uof;

        public ProfileController(ITokenService tokenService, ITokenReceptor tokenReceptor, 
            ImageService imageService, IMapper mapper, IUnitOfWork uof)
        {
            _tokenService = tokenService;
            this.tokenReceptor = tokenReceptor;
            _imageService = imageService;
            _mapper = mapper;
            _uof = uof;
        }

        [AuthenticationUser]
        [HttpPost]
        public async Task<IActionResult> CreateProfile([FromBody] CreateProfileDto request)
        {
            var user = await _tokenService.UserByToken(tokenReceptor.GetToken());

            var image = request.ProfilePicture.OpenReadStream();

            if(request.ProfilePicture is not null)
            {
                _imageService.ValidateImage(image);
            }

            var userProfile = _mapper.Map<ProfileModel>(request);
            userProfile.UserId = user.Id;

            await _uof.profileWriteOnly.AddProfile(userProfile);
            await _uof.Commit();

            var response = _mapper.Map<ResponseProfile>(userProfile);

            return Created(string.Empty, response);
        }
    }
}
