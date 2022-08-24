using Microsoft.AspNetCore.Builder;

namespace northwindq.Subscription.Middleware
{
   static public class DatabaseSubscriptionMiddleware
    {
        public static void UseDatabaseSubscription<T>(this IApplicationBuilder builder,string tableName) where T:class, IDatabaseSubcription
        {
            var subscription =(T)builder.ApplicationServices.GetService(typeof(T));
            subscription.Configure(tableName);
        }
    }
}
