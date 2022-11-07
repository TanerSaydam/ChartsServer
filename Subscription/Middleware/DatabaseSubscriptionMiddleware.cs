namespace ChartsServer.Subscription.Middleware
{
    public static class DatabaseSubscriptionMiddleware
    {
        public static void UseDatabaseSubscription<T>(this IApplicationBuilder app, string tableName) where T : class, IDatabaseSubscription
        {
            var subscription = (T)app.ApplicationServices.GetService(typeof(T));
            subscription.Configure(tableName);
        }
    }
}
