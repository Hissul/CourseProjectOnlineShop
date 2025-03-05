using Client.Models;
using Client.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder (args);

builder.Services.AddRazorPages ();

// ������������ HttpClient ��� ApiService
builder.Services.AddHttpClient<ApiService>(client =>
{
    client.BaseAddress = new Uri ("http://localhost:5242/"); // ����� API
});

// ��������� ��� ��� ������
builder.Services.AddDistributedMemoryCache (); // ��� ���������� ��� �������� ������ ������ � ������

// ��������� ������
builder.Services.AddSession (options => {
    options.IdleTimeout = TimeSpan.FromMinutes (30); // ����� ����� ������
    options.Cookie.HttpOnly = true; // �������� ������
    options.Cookie.IsEssential = true; // ������ ����� ����������� ��� ����������
});

// ������������ IHttpContextAccessor
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor> ();
builder.Services.AddSingleton<UserModel> ();



WebApplication app = builder.Build ();




if (!app.Environment.IsDevelopment ()) {
    app.UseExceptionHandler ("/Error");
    app.UseHsts ();
}

app.UseHttpsRedirection ();
app.UseStaticFiles ();

// ���������� ������
app.UseSession ();
app.UseRouting ();
app.UseAuthentication ();
app.UseAuthorization ();

// ����� ���������
app.MapRazorPages ().WithStaticAssets ();




app.Run ();



