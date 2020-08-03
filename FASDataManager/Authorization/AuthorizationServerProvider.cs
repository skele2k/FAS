using FASDataManager.DataAccessLayer;
using FASLib.Helpers;
using FASLib.Models;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Razor.Generator;

namespace FASDataManager.Authorization
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);

            AdminRepository _adminRepository = new AdminRepository();
            var admins = _adminRepository.GetAdmins();

            bool inValid = false;
            foreach(var admin in admins)
            {
                if (context.UserName == admin.username && context.Password == admin.password)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, "admin"));
                    identity.AddClaim(new Claim("username", "admin"));
                    identity.AddClaim(new Claim(ClaimTypes.Name, admin.username));
                    context.Validated(identity);
                    inValid = true;
                    break;
                }
            }
            if (inValid == false)
            {
                context.SetError("Invalid_grant", "Provided username and password is incorrect.");
            }
        }
        private List<AdminModel> GetAdmins()
        {
            AdminRepository _adminRepository = new AdminRepository();
            return _adminRepository.GetAdmins();
        }
    }
}