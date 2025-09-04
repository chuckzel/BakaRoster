/*
 * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
* See LICENSE in the project root for license information.
*/

using OneRosterProviderDemo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using OneRosterProviderDemo;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApiContext>(
    options => options.UseSqlite(builder.Configuration.GetConnectionString("OneRosterProviderDemoEF"))
);
builder.Services.AddAuthentication(sharedOptions =>
{
    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect(opts =>
{
    builder.Configuration.GetSection("AzureAd").Bind(opts);
    opts.SaveTokens = true;
});
builder.Services.AddOpenApi();

OneRosterProviderDemo.Vocabulary.SubjectCodes.Initialize();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    Console.WriteLine("In Development");
    //app.UseDeveloperExceptionPage();
    app.MapOpenApi();
    app.MapScalarApiReference();
}
Console.WriteLine("bla");

app.UseHttpsRedirection();
//app.UseStaticFiles();
app.UseAuthentication();
//app.UseOauthMessageSigning();
app.UseRouting();
app.UseAuthorization();
/*app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);*/
app.MapControllers();
app.Run();
