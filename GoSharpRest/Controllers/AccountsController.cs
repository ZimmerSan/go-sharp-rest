﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using GoSharpRest.Models;
using GoSharpRest.Models.Constants;
using GoSharpRest.Models.Entities;
using Microsoft.AspNet.Identity;

namespace GoSharpRest.Controllers
{
    [RoutePrefix("api/accounts")]
    public class AccountsController : BaseApiController
    {
        [Authorize]
        [HttpGet]
        [Route("users")]
        public IHttpActionResult GetUsers()
        {
            return Ok(this.AppUserManager.Users.ToList().Select(u => this.TheModelFactory.Create(u)));
        }

        [Authorize]
        [Route("users/{id:guid}", Name = "GetUserById")]
        public async Task<IHttpActionResult> GetUser(string Id)
        {
            var user = await this.AppUserManager.FindByIdAsync(Id);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();

        }

        [Authorize]
        [HttpPut]
        [Route("users/{id:guid}")]
        public async Task<IHttpActionResult> UpdateUser(string id, UserReturnModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            var user = await this.AppUserManager.FindByIdAsync(id);
            if (user != null)
            {
                user.ImageUrl = model.ImageUrl;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Description = model.Description;
                await this.AssignRolesToUser(user.Id, model.Roles.ToArray());

                DB.Entry(user).State = EntityState.Modified;
                await this.DB.SaveChangesAsync();
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();
        }

        [Authorize]
        [Route("users/{id:guid}/full", Name = "GetFullUserById")]
        public async Task<IHttpActionResult> GetUserFull(string Id)
        {
            var user = await this.AppUserManager.FindByIdAsync(Id);

            if (user != null)
            {
                return Ok(this.TheModelFactory.CreateFull(user));
            }

            return NotFound();

        }

        [Authorize]
        [Route("users/{username}")]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            var user = await this.AppUserManager.FindByNameAsync(username);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();

        }

        [AllowAnonymous]
        [HttpPost]
        [Route("users")]
        public async Task<IHttpActionResult> CreateUser(CreateUserBindingModel createUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser()
            {
                UserName = createUserModel.Username,
                Email = createUserModel.Email,
                FirstName = createUserModel.FirstName,
                LastName = createUserModel.LastName,
                JoinDate = DateTime.Now,
            };

            var addUserResult = await this.AppUserManager.CreateAsync(user, createUserModel.Password);
            if (!addUserResult.Succeeded) { return GetErrorResult(addUserResult); }

            var addToRoleResult = this.AppUserManager.AddToRole(user.Id, RoleName.Customer);
            if (!addToRoleResult.Succeeded) { return GetErrorResult(addToRoleResult); }

            var cart = new ShoppingCart()
            {
                User = user,
                Records = new List<CartRecord>()
            };

            DB.Carts.Add(cart);
            DB.SaveChanges();
            Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

            return Created(locationHeader, TheModelFactory.Create(user));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("users/{id:guid}")]
        public async Task<IHttpActionResult> DeleteUser(string id)
        {
            //Only SuperAdmin or Admin can delete users (Later when implement roles)
            var appUser = await this.AppUserManager.FindByIdAsync(id);

            if (appUser != null)
            {
                IdentityResult result = await this.AppUserManager.DeleteAsync(appUser);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }

                return Ok();

            }

            return NotFound();

        }

        [Authorize]
        [Route("changePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await this.AppUserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [Route("users/{id:guid}/roles")]
        [HttpPut]
        public async Task<IHttpActionResult> AssignRolesToUser(string id, [FromBody] string[] rolesToAssign)
        {

            var appUser = await this.AppUserManager.FindByIdAsync(id);

            if (appUser == null)
            {
                return NotFound();
            }

            var currentRoles = await this.AppUserManager.GetRolesAsync(appUser.Id);

            var rolesNotExists = rolesToAssign.Except(this.AppRoleManager.Roles.Select(x => x.Name)).ToArray();

            if (rolesNotExists.Count() > 0)
            {

                ModelState.AddModelError("", string.Format("Roles '{0}' does not exixts in the system", string.Join(",", rolesNotExists)));
                return BadRequest(ModelState);
            }

            IdentityResult removeResult = await this.AppUserManager.RemoveFromRolesAsync(appUser.Id, currentRoles.ToArray());

            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to remove user roles");
                return BadRequest(ModelState);
            }

            IdentityResult addResult = await this.AppUserManager.AddToRolesAsync(appUser.Id, rolesToAssign);

            if (!addResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to add user roles");
                return BadRequest(ModelState);
            }

            return Ok();
        }
    }
}
