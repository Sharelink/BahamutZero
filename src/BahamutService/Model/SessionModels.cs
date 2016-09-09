using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BahamutService.Model
{
    public class AccountSessionData
    {
        public string AccessToken { get; set; }
        public string AppToken { get; set; }
        public string Appkey { get; set; }
        public string AccountId { get; set; }
        public string UserId { get; set; }
    }

    public class DeviceToken
    {
        public const string DeviceTypeIOS = "iOS";
        public const string DeviceTypeAndroid = "Android";
        public const string DeviceTypeWindows = "Windows";
        public string Token { get; set; }
        public string Type { get; set; }

        public bool IsUnValidToken()
        {
            return string.IsNullOrWhiteSpace(Token) || string.IsNullOrWhiteSpace(Type);
        }

        public bool IsValidToken()
        {
            return !IsUnValidToken();
        }

        public bool IsIOSDevice()
        {
            return DeviceTypeIOS == Type;
        }

        public bool IsAndroidDevice()
        {
            return DeviceTypeAndroid == Type;
        }

        public bool IsWindowsDevice()
        {

            return DeviceTypeWindows == Type;
        }
    }

    public class AccessTokenValidateResult
    {
        public bool Succeed
        {
            get
            {
                return null != UserSessionData;
            }
        }
        public AccountSessionData UserSessionData { get; set; }
        public string Message { get; set; }
    }

    public class LoginValidateResult
    {
        public string AccountID { get; set; }
        public string AccountName { get; set; }
        public string ValidatedMobile { get; set; }
        public string ValidatedEmail { get; set; }
        public string Message { get; set; }
        public bool Succeeded { get; set; }
    }

    public interface IAccountSessionData
    {
        AccountSessionData UserSessionData { get; set; }
    }

    public interface IGetAccountSessionData
    {
        AccountSessionData UserSessionData { get; }
    }
}
