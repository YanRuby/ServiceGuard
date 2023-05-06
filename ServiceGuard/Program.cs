﻿using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

// Main Program

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("https://localhost:5000");

var environment = builder.Environment;
var configuration = builder.Configuration;
configuration.SetBasePath(environment.ContentRootPath);
configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
configuration.AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true);
configuration.AddEnvironmentVariables();

#region Add services to the container.

builder.Services.AddControllers();
//builder.Services.AddControllersWithViews(); // 控制器路由視圖服務 index.cshtml

/************************************************
* OpenApi: Swagger >> [/swagger/index.html]
*/
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/************************************************
* 跨域策略
* >> 允許來自不同源(域名、端口或協議)的 HTTP請求
* >> 預設情況瀏覽器會阻擋跨域請求，以避免惡意攻擊
*/
builder.Services.AddCors(options => {
    // 跨域策略名稱: CorsPolicy
    options.AddPolicy("CorsPolicy", builder => {
        builder
        //.WithOrigins(AppSettings.Origins.ToArray())
        .AllowAnyHeader()       // 任何標頭
        .AllowAnyMethod()       // 任何方法
        .AllowCredentials();    // 允許携帶身份驗證信息 (JWT)
    });
});

/************************************************
* JWT 身份驗證
*/
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options => {
    // 是否將詳細錯誤信息傳送給 Client (當驗證失敗時，回應標頭會包含 WWW-Authenticate 標頭，這裡會顯示失敗的詳細錯誤原因)
    options.IncludeErrorDetails = true;
    options.TokenValidationParameters = new TokenValidationParameters {

        // 透過這項宣告，就可以從 "sub" 取值並設定給 User.Identity.Name
        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
        // 透過這項宣告，就可以從 "roles" 取值，並可讓 [Authorize] 判斷角色
        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",

        // JWT 發行者-是否驗證設定
        ValidateIssuer = true,
        ValidIssuer = "https://meowlien.com",

        // JWT 受衆-是否驗證設定 (單一伺服器通常不太需要驗證)
        ValidateAudience = false,
        ValidAudience = "https://meowlien.com",

        // JWT 有效期-是否驗證設定
        ValidateLifetime = true,
        // JWT 過期-是否驗證設定
        RequireExpirationTime = false,

        // JWT 加密鑰匙-是否驗證設定 (如果 Token 中包含 key 才需要驗證，一般都只有簽章而已)
        ValidateIssuerSigningKey = false,
        // JWT 加密鑰匙-設定
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1234567890123456"))
    };
});

/************************************************
* 資料庫
*/
// pgsql
//builder.Services.AddEntityFrameworkNpgsql().AddDbContext<Models.O2OEntities>();
// mssql

var app = builder.Build();
#endregion

#region Configure the HTTP request pipeline.

// 是否在開發環境中運行
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage(); // 提供錯誤詳細訊息
    app.UseSwagger();                // 啓用-Swagger API 測試頁面
    app.UseSwaggerUI();              // '''
}

/************************************************
* IApplicationBuilder
*/
app.UseHttpsRedirection();        // 啓用-HTTPS導向功能 (將HTTP重新導向到HTTPS)
//app.UseStaticFiles();             // 啓用-靜態文件
app.UseRouting();                 // 啓用-URL路由
app.UseCors("CorsPolicy");        // 啓用-跨域策略
app.UseAuthentication();          // 啓用-身份驗證
app.UseAuthorization();           // 啓用-授權功能

/************************************************
* IEndpointRouterBuilder
*/

app.MapControllers();               // 將控制器類型映射到 URL 路由       [Services >> AddControllers()]
app.MapControllerRoute(             // 預設 URL 路由頁面 index.cshtml   [Services >> AddControllers() | AddControllersWithViews()]
    name: "default",
    pattern: "{controller}/{action}/{id?}",
    defaults: new { controller = "Home", action = "Index" }
);

/************************************************
* IHostBuilder
*/
app.Run();
#endregion