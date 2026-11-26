using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Projeto_Bibliotec_MVC_tarde.Data;
using Projeto_Bibliotec_MVC_tarde.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ConexaoPadrao")
    )    
);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Página para qual o usuário será direcionado caso não esteja autenticado.
        options.LoginPath = "/Login/Index";

        // Página para qual o usuário será direcionado caso não tenha permissão
        options.AccessDeniedPath = "/Login/AcessoNegado";

        // Define o tempo de validade da autenticação
        options.ExpireTimeSpan = TimeSpan.FromHours(2);

        // Renova a validade da autenticação conforme a atividade do usuário
        options.SlidingExpiration = true;
    });

// Registra os recursos de autorização.
builder.Services.AddAuthorization();

// Registra o serviço responsável pela geração e validação de hash de senhas
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();



var app = builder.Build();


//!
// Código temporário para atualizar as senhas de teste.
using (var scope = app.Services.CreateScope())
{
    // Obtém o contexto do banco de dados.
    var contexto = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Obtém o serviço responsável pela geração do hash.
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>();

    // Busca os usuários que ainda possuem a senha de teste em texto puro.
    var usuarios = contexto.Usuario
        .Where(u => u.Senha == "123456")
        .ToList();

    foreach (var usuario in usuarios)
    {
        // Gera o hash da senha do usuário.
        usuario.Senha = passwordHasher.HashPassword(usuario, usuario.Senha);
    }

    // Salva as alterações no banco de dados.
    contexto.SaveChanges();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// A ordem é importante -> primeiro identifica o usuário, depois ve se ele tem permissão
// primeiro -> UseAuthentication();
// depois -> UseAuthorization();

// Identifica o usuário do cookie de autenticação
app.UseAuthentication();

// Verifica se o usuário possui permissão para acessar o recurso solicitado.
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
