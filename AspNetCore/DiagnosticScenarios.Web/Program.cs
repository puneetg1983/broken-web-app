namespace DiagnosticScenarios.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            //
            // Intentionally throw an exception to simulate a startup failure when CRASH_ON_STARTUP
            // environment variable is set to true.
            //
            if (Environment.GetEnvironmentVariable("CRASH_ON_STARTUP") == "true")
            {
                throw new InvalidOperationException("I am failing at startup! Catch me if you can!");
            }

            app.Run();
        }
    }
}
