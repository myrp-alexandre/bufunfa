﻿using JNogueira.Bufunfa.Api;
using JNogueira.Bufunfa.Api.Filters;
using JNogueira.Bufunfa.Dominio.Interfaces.Dados;
using JNogueira.Bufunfa.Dominio.Interfaces.Servicos;
using JNogueira.Bufunfa.Dominio.Servicos;
using JNogueira.Bufunfa.Infraestrutura.Dados;
using JNogueira.Bufunfa.Infraestrutura.Dados.Repositorios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Text;

namespace Bufunfa.Api
{
    public class Startup
    {
        public static IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            // Configuração realizada, seguindo o artigo "ASP.NET Core 2.0: autenticação em APIs utilizando JWT" 
            // (https://medium.com/@renato.groffe/asp-net-core-2-0-autentica%C3%A7%C3%A3o-em-apis-utilizando-jwt-json-web-tokens-4b1871efd)

            var tokenConfig = new TokenJwtConfig();

            // Extrai as informações do arquivo appsettings.json, criando um instância da classe "TokenJwtConfig"
            new ConfigureFromConfigurationOptions<TokenJwtConfig>(Configuration.GetSection("TokenJwtConfig"))
                .Configure(tokenConfig);

            // AddSingleton: instância configurada de forma que uma única referência das mesmas seja empregada durante todo o tempo em que a aplicação permanecer em execução
            services.AddSingleton(tokenConfig);

            services
                 // AddAuthentication: especificará os schemas utilizados para a autenticação do tipo Bearer
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                // AddJwtBearer: definidas configurações como a chave e o algoritmo de criptografia utilizados, a necessidade de analisar se um token ainda é válido e o tempo de tolerância para expiração de um token
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // Valida a assinatura de um token recebido
                        ValidateIssuerSigningKey = true,
                        // Verifica se um token recebido ainda é válido
                        ValidateLifetime = true,
                        ValidIssuer = tokenConfig.Issuer,
                        ValidAudience = tokenConfig.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenConfig.SecretKey)),
                        // Tempo de tolerância para a expiração de um token (utilizado
                        // caso haja problemas de sincronismo de horário entre diferentes
                        // computadores envolvidos no processo de comunicação)
                        ClockSkew = TimeSpan.Zero
                    };
                });

            // AddAuthorization: ativará o uso de tokens com o intuito de autorizar ou não o acesso a recursos da aplicação
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Bearer", 
                    new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .Build());
            });

            services.AddMvc(config =>
            {
                config.Filters.Add(typeof(CustomExceptionFilter));
            });

            services.AddScoped<BufunfaDataContext, BufunfaDataContext>(x => new BufunfaDataContext(Configuration["BufunfaConnectionString"]));

            // AddTransient: determina que referências desta classe sejam geradas toda vez que uma dependência for encontrada
            services.AddTransient<IUsuarioRepositorio, UsuarioRepositorio>();

            services.AddTransient<IUsuarioServico, UsuarioServico>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseMvc();
        }
    }
}
