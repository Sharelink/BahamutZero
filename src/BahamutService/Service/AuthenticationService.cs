﻿using System.Threading.Tasks;
using System.Linq;
using System;
using BahamutService.Model;
using BahamutCommon;
using System.Text.RegularExpressions;

namespace BahamutService
{
    public class AuthenticationService
    {
        protected BahamutDBContext DBContext { get { return new BahamutDBContext(connectionString); } }
        protected string connectionString { get; set; }

        public AuthenticationService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public bool CheckUsernamePasswordIsValid(string username, string password)
        {
            return Regex.IsMatch(username, @"^[_a-zA-Z0-9\u4e00-\u9fa5]{2,23}$") && Regex.IsMatch(password, @"^[a-zA-Z0-9]{16,}$");
        }

        public LoginValidateResult LoginValidate(string loginString, string password)
        {
            return Validate(loginString, password);
        }

        private LoginValidateResult Validate(string loginString, string password)
        {
            if (!CheckUsernamePasswordIsValid(loginString,password))
            {
                throw new Exception("VALIDATE_INFO_INVALID");
            }
            else
            {
                var accounts = from a in DBContext.Account where (a.AccountID.ToString() == loginString || a.AccountName == loginString || a.Mobile == loginString || a.Email == loginString) && a.Password == password select a;
                if (accounts.Count() > 0)
                {
                    var account = accounts.First();
                    return new LoginValidateResult()
                    {
                        AccountID = account.AccountID.ToString(),
                        Message = "Yes",
                        Succeeded = true,
                        AccountName = account.AccountName,
                        ValidatedEmail = account.Email,
                        ValidatedMobile = account.Mobile
                    };
                }
                else
                {
                    throw new NullReferenceException("VALIDATE_FAILED");
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
