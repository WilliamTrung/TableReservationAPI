using ApplicationService.Models;
using ApplicationService.Models.UserModels;
using ApplicationService.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.Net;
using System.Xml.Linq;

namespace TableReservationAPI.CustomMiddleware
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class GoogleAuthorized : ActionFilterAttribute, IActionFilter
    {
        private bool isAuthorized = true;
        private bool _requiredPhone;
        private List<EnumModel.Role> _roles = new List<EnumModel.Role>();
        private IAccountService _loginService = null!;
        public GoogleAuthorized(string roles, bool requiredPhone = false)
        {
            var _roles_split = roles.Split(',');
            _roles = _roles_split.Select(str => (EnumModel.Role)Enum.Parse(typeof(EnumModel.Role), str)).ToList();
            _requiredPhone = requiredPhone;
        }
        public GoogleAuthorized(bool requiredPhone = false)
        {
            _requiredPhone = requiredPhone;    
        }
        private void SetConfiguration(ActionExecutingContext context)
        {
            var serviceProvider = context.HttpContext.RequestServices;
            _loginService = (IAccountService)serviceProvider.GetRequiredService(typeof(IAccountService));
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string authHeader = context.HttpContext.Request.Headers["Authorization"];
            Console.WriteLine(authHeader);
            SetConfiguration(context);
            AuthorizedModel? authorized = null;
            string message = "Unauthorized";
            try
            {
                authorized = _loginService.ValidateLoginAsync(authHeader).Result;
                bool roleAuthorized = true;
                bool phoneAuthorized = true;
                if (_roles.Count > 0 && !_roles.Any(r => r == (EnumModel.Role)authorized.Role))
                {
                    //role authorizing
                    roleAuthorized = false;
                    message += " - Unauthorized for this function!";
                }
                if(_requiredPhone && authorized.Phone == null)
                {
                    //phone authorizing
                    phoneAuthorized = false;
                    message += " - Phone required";
                }
                isAuthorized = roleAuthorized && phoneAuthorized;
                if (!isAuthorized)
                {
                    if (_roles.Count > 0)
                    {
                        message += " - Required Role(s): " + GetAlertRequiredRoles();
                        if (authorized != null)
                        {
                            message += " - Current role: " + authorized.Role.ToString();
                        }
                    }
                }
            }
            catch (Exception)
            {
                isAuthorized = false;
            }
            if (!isAuthorized)
            {
                         
                context.Result = context.Result = new ObjectResult(message)
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized
                };
                return;
            }
        }
        private string GetAlertRequiredRoles()
        {
            string result = "";
            foreach (var item in _roles)
            {
                result += item + ",";
            }
            result = result.Substring(0, result.Length - 1);
            return result;
        }
    }
}
