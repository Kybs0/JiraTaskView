using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kybs0.Net.Utils;

namespace JiraTask.Business
{
    public class UserSettingConfigHelper
    {
        private const string UserSetting = "UserSetting";
        private const string ProjectKey = "Project";
        private const string AccountKey = "Account";
        private const string PasswordKey = "Password";
        public static UserSetting GetUserSetting()
        {
            var projectKey = IniFileHelper.IniReadValue(UserSetting, ProjectKey);
            var accountKey = IniFileHelper.IniReadValue(UserSetting, AccountKey);
            var passwordKey = IniFileHelper.IniReadValue(UserSetting, PasswordKey);
            return new UserSetting()
            {
                ProjectName = projectKey,
                Account = accountKey,
                Password = passwordKey
            };
        }

        public static void SetUserSetting(UserSetting userSetting)
        {
            IniFileHelper.IniWriteValue(UserSetting, ProjectKey, userSetting.ProjectName);
            IniFileHelper.IniWriteValue(UserSetting, AccountKey, userSetting.Account);
            IniFileHelper.IniWriteValue(UserSetting, PasswordKey, userSetting.Password);
        }

        public static bool IsLogined()
        {
            var userSetting = GetUserSetting();

            if (!string.IsNullOrEmpty(userSetting.ProjectName) &&
                !string.IsNullOrEmpty(userSetting.Account) &&
                !string.IsNullOrEmpty(userSetting.Password))
            {
                return true;
            }

            return false;
        }
    }

    public class UserSetting
    {
        public string ProjectName { get; set; } = "E"+ "N";
        public string Account { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
