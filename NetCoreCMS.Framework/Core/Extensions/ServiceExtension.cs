﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCMS.Framework.Core.Models;
using NetCoreCMS.Framework.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using NetCoreCMS.Framework.Core.Data;
using NetCoreCMS.Framework.Core.Auth;

namespace NetCoreCMS.Framework.Core.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddCustomizedIdentity(this IServiceCollection services)
        {
            services.AddIdentity<NccUser, NccRole>(
                configure => {                
                    configure.Password.RequireDigit = false;
                    configure.Password.RequireLowercase = false;
                    configure.Password.RequireNonAlphanumeric = false;
                    configure.Password.RequireUppercase = false;
                    configure.Password.RequiredLength = 1;
                    configure.Lockout.MaxFailedAccessAttempts = 5;
                    configure.SignIn.RequireConfirmedEmail = false;
                    configure.SignIn.RequireConfirmedPhoneNumber = false;
                }
            )
            .AddEntityFrameworkStores<NccDbContext>()
            .AddRoleStore<NccRoleStore>()
            .AddUserStore<NccUserStore>()
            .AddDefaultTokenProviders();
            
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options => {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logoff";
            });

            services.ConfigureApplicationCookie(options => options.LoginPath = "/Account/Login");

            /*
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o => o.LoginPath = new PathString("/Account/login"))

                .AddFacebook(x =>
                {
                    x.AppId = "";
                    x.AppSecret = "";

                    x.Events = new OAuthEvents
                    {
                        OnRemoteFailure = ctx => HandleRemoteLoginFailure(ctx)
                    };
                })
                .AddGoogle(x =>
                {
                    x.ClientId = "";
                    x.ClientSecret = "";
                    x.Events = new OAuthEvents
                    {
                        OnRemoteFailure = ctx => HandleRemoteLoginFailure(ctx)
                    };
                });
            */

            return services;
        }

        private static Task HandleRemoteLoginFailure(RemoteFailureContext ctx)
        {
            ctx.Response.Redirect("/Account/Login");
            ctx.HandleResponse();
            return Task.CompletedTask;
        }

        public static IServiceProvider Build(this IServiceCollection services, IConfigurationRoot configuration, IHostingEnvironment hostingEnvironment)
        {
            var builder = new ContainerBuilder();
            
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();
            builder.Register<SingleInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            builder.Register<MultiInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => (IEnumerable<object>)c.Resolve(typeof(IEnumerable<>).MakeGenericType(t));
            });

            foreach (var module in GlobalConfig.Modules)
            {
                builder.RegisterAssemblyTypes(module.Assembly).AsImplementedInterfaces();
                services.AddMediatR(module.Assembly);
            }

            builder.RegisterInstance(configuration);
            builder.RegisterInstance(hostingEnvironment);
            builder.Populate(services);
            var container = builder.Build();
            
            return container.Resolve<IServiceProvider>();
        }
    }
}
