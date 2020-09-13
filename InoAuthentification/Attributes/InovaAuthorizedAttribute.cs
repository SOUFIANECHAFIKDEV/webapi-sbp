using InoAuthentification.DbContexts;
using InoAuthentification.Helpers;
using InoAuthentification.JwtManagers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web.Http;

namespace InoAuthentification.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class InovaAuthorizedAttribute : Attribute
    {

        public InovaAuthorizedAttribute(string className, [CallerMemberName] string membername = "")
        {
            StringValues authorizationToken;
            InovaHelper.HttpCurrent.Request.Headers.TryGetValue("Authorization", out authorizationToken);
            JwtManager jwtManager = new JwtManager();
            var tokenValidation = jwtManager.VerifyingToken(authorizationToken);
            if (tokenValidation.IsValid)
            {
                using (var dbContext = new InoAuthentificationDbContext(InovaHelper._dbContextOptions))
                {
                    var user = dbContext.User.Where(x => x.Id == tokenValidation.CurrentToken.UserId).FirstOrDefault();
                    if (user == null)
                    {
                        var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Oops!!!" };
                        throw new HttpResponseException(msg);
                    }
                    var profileActions = from up in dbContext.UserProfile
                                         join pf in dbContext.Profile on up.Idprofile equals pf.Id
                                         join pa in dbContext.ProfileAction on pf.Id equals pa.Idprofile
                                         join ac in dbContext.Action on pa.Idaction equals ac.Id
                                         join mdl in dbContext.Module on ac.IdModule equals mdl.Id
                                         where
                                         up.Iduser == tokenValidation.CurrentToken.UserId
                                         && ac.CodeName == membername
                                         && mdl.Libelle == className
                                         select pa;
                    var isAuthprized = profileActions.Count() > 0;

                    if (!isAuthprized)
                    {
                        var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Oops!!!" };
                        throw new HttpResponseException(msg);
                    }
                }
            }
            else
            {
                var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Oops!!!" };
                throw new HttpResponseException(msg);
            }
        }
    }
}
