namespace UserManagmentRazor.Extentions
{
    public static class ServiceRegiteration
    {
//        public static void RegisterService(WebApplicationBuilder builder)
//        {
//            builder.Services.AddHttpClient<IGenericApiService, GenericApiService>(client =>
//{
//    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]); // base URL from appsettings.json
//    client.DefaultRequestHeaders.Add("Accept", "application/json");
//});
//        }

        public static void RegisterService(WebApplicationBuilder builder)
        {
            // Register HttpClient with a configuration lambda (no return value needed)
            builder.Services.AddHttpClient<IGenericApiService, GenericApiService>(client =>
            {
                // Set base URL and headers
                client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]); // Base URL from appsettings.json
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
        }

    }
}
