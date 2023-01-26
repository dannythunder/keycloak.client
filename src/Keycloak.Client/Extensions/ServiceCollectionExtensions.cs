﻿using System;

using Keycloak.Client.Models;

using Microsoft.Extensions.DependencyInjection;

using Refit;

namespace Keycloak.Client.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKeycloakClient(this IServiceCollection services, Action<KeycloakOptions> configure)
        {
            var options = new KeycloakOptions();
            configure(options);
            services.AddSingleton(options);

            services
                .AddRefitClient<IKeycloakAuthClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{options.KeycloakBasePath}/realms/{(string.IsNullOrEmpty(options.AuthRealm) ? "master" : options.AuthRealm)}"));

            services.AddTransient<KeycloakServiceUserAuthHandler>();
            services.AddSingleton<IKeycloakAuthTokenStore, KeycloakAuthTokenStore>();

            services
                .AddRefitClient<IKeycloakUserClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{options.KeycloakBasePath}/admin/realms/{options.Realm}"))
                .AddHttpMessageHandler<KeycloakServiceUserAuthHandler>();

            services
                .AddRefitClient<IKeycloakEventClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri($"{options.KeycloakBasePath}/admin/realms/{options.Realm}"))
                .AddHttpMessageHandler<KeycloakServiceUserAuthHandler>();

            return services;
        }
    }
}
