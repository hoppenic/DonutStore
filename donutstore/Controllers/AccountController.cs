using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using donutstore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Authorization;


namespace donutstore.Controllers
{
    public class AccountController : Controller     
    {
        SignInManager<DonutStoreUser> _signInManager;
        EmailService _emailService;

        //using Microsoft.AspNetCore.Identity
        public AccountController(SignInManager<DonutStoreUser> signInManager, EmailService emailService)
        {
            this._signInManager = signInManager;
            this._emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Responds on GET /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // Responds on POST /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                DonutStoreUser newUser = new DonutStoreUser
                {
                    UserName = model.UserName,
                    Email=model.Email,
                    FirstName=model.FirstName,
                    LastName=model.LastName,
                    PhoneNumber=model.PhoneNumber
                };

                IdentityResult creationResult = this._signInManager.UserManager.CreateAsync(newUser).Result;
                if (creationResult.Succeeded)
                {
                    IdentityResult passwordResult = this._signInManager.UserManager.AddPasswordAsync(newUser, model.Password).Result;
                    if (passwordResult.Succeeded)
                    {
                        var confirmationToken = _signInManager.UserManager.GenerateEmailConfirmationTokenAsync(newUser).Result;

                        confirmationToken = System.Net.WebUtility.UrlEncode(confirmationToken);
                        string currentUrl = Request.GetDisplayUrl();
                        System.Uri uri = new Uri(currentUrl);
                        string confirmationUrl = uri.GetLeftPart(UriPartial.Authority);
                        confirmationUrl += "/account/confirm?id=" + confirmationToken + "&userId=" + System.Net.WebUtility.UrlEncode(newUser.Id);
                        this._signInManager.SignInAsync(newUser, false);

                        var emailResult =this._emailService.SendEmailAsync(model.Email, "Welcome to Flavor Town Burgers", "<p> Thanks for signing up, " + model.Email + "!</p><p>< a href =\"" + confirmationUrl + "\">Confirm your account<a></p>", "Thanks for signing up, " + model.Email);
                        if (emailResult.IsCompletedSuccessfully)
                            return RedirectToAction("Index", "Home");
                        else
                            return BadRequest(emailResult.IsCanceled);
                        
                    }
                    else
                    {
                        this._signInManager.UserManager.DeleteAsync(newUser).Wait();
                        foreach (var error in passwordResult.Errors)
                        {
                            ModelState.AddModelError(error.Code, error.Description);
                        }
                    }
                        
                }
                else
                {
                    foreach (var error in creationResult.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                }
                   
            }
                
            
            return View();
        }

        public IActionResult SignOut()
        {
            this._signInManager.SignOutAsync().Wait();
            return RedirectToAction("Index", "Home");
        }


        //responds on GET
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignIn(SignInViewModel model)
        {
            if (ModelState.IsValid)
            {

                DonutStoreUser existingUser = this._signInManager.UserManager.FindByNameAsync(model.UserName).Result;
                if (existingUser != null)
                {
                    Microsoft.AspNetCore.Identity.SignInResult passwordResult = this._signInManager.CheckPasswordSignInAsync(existingUser, model.Password, false).Result;
                    if (passwordResult.Succeeded)
                    {
                        this._signInManager.SignInAsync(existingUser, false).Wait();
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("PasswordIncorrect", "Username or Password is incorrect.");
                    }
                }
                else
                {
                    ModelState.AddModelError("UserDoesNotExist", "Username or Password is incorrect.");

                }
            }
            return View();
        }


        //get
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string userName)
        {
            if ((ModelState.IsValid) && (!string.IsNullOrEmpty(userName)))
            {
                var user = await _signInManager.UserManager.FindByEmailAsync(userName);
                if (user != null)
                {
                    var resetToken = await _signInManager.UserManager.GeneratePasswordResetTokenAsync(user);

                    resetToken = System.Net.WebUtility.UrlEncode(resetToken);
                    string currentUrl = Request.GetDisplayUrl(); //gets url for current request
                    System.Uri uri = new Uri(currentUrl); //wraps it in a 'uri' object so I can split it into parts
                    string resetUrl = uri.GetLeftPart(UriPartial.Authority); //gives me the scheme + authority of the URL
                    resetUrl += "/account/resetpassword?id=" + resetToken + "&userId=" + System.Net.WebUtility.UrlEncode(user.Id);


                    string htmlContent = "<a href=\"" + resetUrl + "\">Reset your password</a>";
                    var emailResult = await _emailService.SendEmailAsync(userName, "Reset your password", htmlContent, resetUrl);
                    if (emailResult.Success)
                    {
                        return RedirectToAction("ResetSent");
                    }
                    else
                    {
                        return BadRequest(emailResult.Message);
                    }
                }
            }
            ModelState.AddModelError("email", "Email is not valid");
            return View();

        }

        public IActionResult ResetSent()
        {
            return View();

        }

        public IActionResult ResetPassword()
        {
            return View();
        }













    }



}







