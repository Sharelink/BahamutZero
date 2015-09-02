using System.Threading.Tasks;
using System.Linq;
using System;
using BahamutService.Model;
using BahamutCommon;

namespace BahamutService
{
    public class AuthenticationService
    {
        protected BahamutDBContext DBContext { get; private set; }

        public AuthenticationService(string connectionString)
            :this(new BahamutDBContext(connectionString))
        {
        }

        public AuthenticationService(BahamutDBContext DBContext)
        {
            this.DBContext = DBContext;
        }

        public LoginValidateResult LoginValidate(string loginString, string password)
        {
            return Validate(loginString, password);
        }

        private LoginValidateResult Validate(string loginString, string password)
        {
            if (string.IsNullOrWhiteSpace(loginString))
            {
                throw new Exception("loginInfo is empty");
            }
            else if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("password is empty");
            }
            else
            {
                var accounts = from a in DBContext.Account where (a.AccountID.ToString() == loginString || a.Mobile == loginString || a.Email == loginString) && a.Password == password select a;
                if (accounts.Count() > 0)
                {
                    var account = accounts.First();
                    return new LoginValidateResult() { AccountID = account.AccountID.ToString(), Message = "Yes", Succeeded = true };
                }
                else
                {
                    throw new NullReferenceException("Account Not Exists Or Password Wrong");
                }
            }
        }
        
    }

    public static class AuthenticationServiceExtensions
    {
        public static IBahamutServiceBuilder UseAuthenticationService(this IBahamutServiceBuilder builder, params object[] args)
        {
            return builder.UseService<AuthenticationService>(args);
        }

        public static AuthenticationService GetAuthenticationService(this IBahamutServiceBuilder builder)
        {
            return builder.GetService<AuthenticationService>();
        }
    }
}
