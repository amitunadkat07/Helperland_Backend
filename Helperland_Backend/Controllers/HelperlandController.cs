﻿using Helperland.Entity.DataModels;
using Helperland.Entity.Model;
using Helperland.Repository.Interface;
using Helperland.Repository.TokenService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Helperland.Controllers
{
    [Route("api/Helperland")]
    [ApiController]
    public class HelperlandController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<HelperlandController> _logger;

        public HelperlandController(IUserService userService, ITokenService tokenService, ILogger<HelperlandController> logger)
        {
            _userService = userService;
            _tokenService = tokenService;
            _logger = logger;
        }

        #region Login
        [HttpPost, Route("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<LoginModel> Login([FromBody] LoginModel user)
        {
            try
            {
                UserDataModel userData = _userService.Login(user);
                if (!userData.IsError)
                {
                    var jwtToken = _tokenService.GenerateJWTAuthetication(userData);
                    userData.Token = jwtToken;
                    return Ok(userData);
                }
                string errorMessage = "Unauthorized user " + user.Email + " tried to login";
                _logger.LogError(errorMessage);
                return NotFound(userData);
            }
            catch
            {
                return NotFound();
            }
        }
        #endregion

        #region Signup
        [HttpPost, Route("Signup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<UserModel> Signup([FromBody] UserModel user)
        {
            try
            {
                UserDataModel userData = _userService.Signup(user);
                if (!userData.IsError)
                {
                    if (userData.RoleId == 2)
                    {
                        var jwtToken = _tokenService.GenerateJWTAuthetication(userData);
                        userData.Token = jwtToken;
                    }
                    return Ok(userData);
                }
                else
                {
                    string errorMessage = user.Firstname + " " + user.Lastname + " tried to register again";
                    _logger.LogError(errorMessage);
                    return BadRequest(userData);
                }
            }
            catch
            {
                return BadRequest();
            }
        }
        #endregion

        #region ForgotPass
        [HttpPost, Route("ForgotPass")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ResetPass> ForgotPass([FromBody] ResetPass user)
        {
            try
            {
                ResetPass passwordObject = _userService.ForgotPass(user);
                if (!passwordObject.IsError)
                {
                    return Ok(passwordObject);
                }
                string errorMessage = "Unauthorized user " + user.Email + " wants to change the password";
                _logger.LogError(errorMessage);
                return NotFound(passwordObject);
            }
            catch 
            { 
                return BadRequest(); 
            }
        }
        #endregion

        #region ResetPassLink
        [HttpPost, Route("ResetPassLink")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ResetPass> ResetPassLink([FromBody] ResetPass user)
        {
            try
            {
                ResetPass passwordObject = _userService.ResetPassLink(user);
                if (!passwordObject.IsError)
                {
                    return Ok(passwordObject);
                }
                return NotFound(passwordObject);
            }
            catch
            {
                return NotFound();
            }
        }
        #endregion

        #region ResetPass
        [HttpPost, Route("ResetPass")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ResetPass> ResetPass([FromBody] ResetPass user)
        {
            try
            {
                ResetPass passwordObject = _userService.ResetPass(user);
                if (!passwordObject.IsError)
                {
                    return Ok(passwordObject);
                }
                return NotFound(passwordObject);
            }
            catch
            {
                return NotFound();
            }
        }
        #endregion

        #region GetUsers
        [Authorize]
        [HttpGet, Route("GetUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<UserDataModel>> GetUsers()
        {
            try
            {
                List<UserDataModel> user = _userService.GetUsers();
                return Ok(user);
            }
            catch
            {
                return Unauthorized();
            }
        }
        #endregion

        #region GetProfile
        [Authorize]
        [HttpGet("GetProfile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ProfileDataModel> GetProfile(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest();
                }
                ProfileDataModel profile = _userService.GetProfile(email);
                if (profile.Email == null)
                {
                    return NotFound(profile);
                }
                return Ok(profile);
            }
            catch
            {
                return Unauthorized();
            }
        }
        #endregion

        #region UpdateProfile
        [Authorize]
        [HttpPut("UpdateProfile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<ProfileDataModel> UpdateProfile(ProfileDataModel profile)
        {
            try
            {
                ProfileDataModel profileDataModel = _userService.UpdateProfile(profile);
                if (profileDataModel.Email == null)
                {
                    return BadRequest(profileDataModel);
                }
                return Ok(profileDataModel);
            }
            catch
            {
                return Unauthorized();
            }
        }
        #endregion

        #region ChangePassword
        [Authorize]
        [HttpPut("UpdatePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<PasswordModel> UpdatePassword(PasswordModel password)
        {
            try
            {
                PasswordModel passwordModel = _userService.UpdatePassword(password);
                if (passwordModel.Email == null)
                {
                    return BadRequest(passwordModel);
                }
                return Ok(passwordModel);
            }
            catch
            {
                return Unauthorized();
            }
        }
        #endregion

        #region CreateAddress
        [HttpPost, Route("CreateAddress")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<AddressDataModel> CreateAddress([FromBody] AddressDataModel address)
        {
            try
            {
                AddressDataModel addressDataModel = _userService.CreateAddress(address);
                if (addressDataModel.IsError)
                {
                    return BadRequest(addressDataModel);
                }
                else
                {
                    return Ok(addressDataModel);
                }
            }
            catch (Exception ex) 
            {
                return Unauthorized(ex);
            }
        }
        #endregion

        #region UpdateAddress
        [HttpPut, Route("UpdateAddress")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<AddressDataModel> UpdateAddress(AddressDataModel address)
        {
            try
            {
                if (address.AddressId == null)
                {
                    return BadRequest();
                }
                else
                {
                    AddressDataModel addressDataModel = _userService.UpdateAddress(address);
                    if (addressDataModel.IsError)
                    {
                        return BadRequest(addressDataModel);
                    }
                    else
                    {
                        return Ok(addressDataModel);
                    }
                }
            }
            catch
            {
                return Unauthorized();
            }
        }
        #endregion

        #region GetAddressByUser
        [Authorize]
        [HttpGet, Route("GetAddressByUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<AddressDataModel>> GetAddressByUser(string email)
        {
            try
            {
                List<AddressDataModel> addressData = _userService.GetAddressByUser(email);
                return Ok(addressData);
            }
            catch
            {
                return Unauthorized();
            }
        }
        #endregion

        #region GetAddressById
        [Authorize]
        [HttpGet, Route("GetAddressById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<AddressDataModel> GetAddressById(int id)
        {
            try
            {
                AddressDataModel addressData = _userService.GetAddressById(id);
                if (addressData.IsError)
                {
                    return BadRequest(addressData);
                }
                return Ok(addressData);
            }
            catch
            {
                return Unauthorized();
            }
        }
        #endregion

        #region DeleteAddress
        [Authorize]
        [HttpDelete, Route("DeleteAddress")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteAddress(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                if (!_userService.DeleteAddress(id))
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch
            {
                return Unauthorized();
            }
        }
        #endregion
    }
}
