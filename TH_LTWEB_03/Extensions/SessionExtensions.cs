using Microsoft.AspNetCore.Mvc;
using TH_LTWEB_03.Models;
using TH_LTWEB_03.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using TranTrungVietHoang.Models;
using System.Text.Json;



namespace TranTrungVietHoang.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }
        public static T GetObjectFromJson<T>(this ISession session,
        string key)
        {
            var value = session.GetString(key);
            return value == null ? default :
            JsonSerializer.Deserialize<T>(value);
        }
    }
}
