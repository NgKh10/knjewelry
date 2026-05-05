using Newtonsoft.Json;
using knjewelry.Models.ViewModels;

namespace knjewelry.Repository
{
    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var data = session.GetString(key);
            return data == null ? default(T) : JsonConvert.DeserializeObject<T>(data);
        }

        public static void SetCartSummary(this ISession session, int count, decimal total)
        {
            session.SetInt32("CartCount", count);
            session.SetString("CartTotal", total.ToString());
        }

        public static int GetCartCount(this ISession session)
        {
            return session.GetInt32("CartCount") ?? 0;
        }

        public static void ClearCartSummary(this ISession session)
        {
            session.Remove("CartCount");
            session.Remove("CartTotal");
        }
    }
}