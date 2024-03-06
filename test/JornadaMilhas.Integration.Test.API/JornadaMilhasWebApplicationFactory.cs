﻿using Bogus;
using JornadaMilhas.API.DTO.Auth;
using JornadaMilhas.Dados;
using JornadaMilhas.Dominio.Entidades;
using JornadaMilhas.Dominio.ValueObjects;
using JornadaMilhas.Integration.Test.API.DataBuilders;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Writers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JornadaMilhas.Integration.Test.API;
public class JornadaMilhasWebApplicationFactory : WebApplicationFactory<Program>
{
    private IServiceScope scope;

    public JornadaMilhasWebApplicationFactory()
    {
        this.scope = Services.CreateScope();
        Context = scope.ServiceProvider.GetRequiredService<JornadaMilhasContext>();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<JornadaMilhasContext>));

            services.AddDbContext<JornadaMilhasContext>(options =>
                        options
                        .UseLazyLoadingProxies()
                        .UseSqlServer("Server=localhost,11433;Database=JornadaMilhasV3;User Id=sa;Password=Alura#2024;Encrypt=false;TrustServerCertificate=true;MultipleActiveResultSets=true;"));

        });
        return base.CreateHost(builder);
    }

    public JornadaMilhasContext Context { get; }

    public async Task<HttpClient> GetClientWithAccessTokenAsync()
    {
        var client = this.CreateClient();

        var user = new UserDTO { Email = "tester@email.com", Password = "Senha123@" };        
        var resultado = await client.PostAsJsonAsync("/auth-login", user);

        resultado.EnsureSuccessStatusCode();

        var result = await resultado.Content.ReadFromJsonAsync<UserTokenDTO>();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result!.Token);

        return client;
    }
}
