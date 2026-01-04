namespace EntreLaunch.Web.Extensions
{
    public static class AppPipelineExtensions
    {
        public static void UseAppPipeline(this WebApplication app)
        {
            app.UseHttpsRedirection();
            app.UseExceptionHandler("/error");
            app.UseForwardedHeaders();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseCors();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<CultureMiddleware>();
            app.UseMiddleware<PermissionMiddleware>();

            app.Use(async (context, next) =>
            {
                Console.WriteLine($"Request Origin: {context.Request.Headers["Origin"]}");
                await next.Invoke();
            });

            app.UseCookiePolicy();

            app.MapControllers();
        }
    }
}
