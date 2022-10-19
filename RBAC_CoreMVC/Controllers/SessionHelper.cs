using Microsoft.AspNetCore.Http;

namespace RBAC_CoreMVC.Controllers
{
    /// <summary>
    /// session
    /// </summary>
    public class SessionHelper
    {
        /// <summary>
        /// 设置Session
        /// </summary>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetSession(ISession session, string key, string value)
        {
            session.SetString(key, value);
        }

        /// <summary>
        /// 获取Session
        /// </summary>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetSession(ISession session, string key)
        {
            var value = session.GetString(key);
            if (string.IsNullOrEmpty(value))
            {
                value = string.Empty;
            }

            return value;
        }

        /// <summary>
        /// 删除Session
        /// </summary>
        /// <param name="session"></param>
        /// <param name="key"></param>
        public static void RemoveSession(ISession session, string key)
        {
            session.Remove(key);
        }
    }
}
