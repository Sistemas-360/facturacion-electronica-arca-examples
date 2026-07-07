using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Sistemas360Example.Api.Client;
using Sistemas360Example.Api.Configuration;
using Sistemas360Example.Api.Services;

var builder =
    WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
    .AddOptions<Sistemas360Options>()
    .Bind(
        builder.Configuration.GetSection(
            Sistemas360Options.SectionName
        )
    )
    .Validate(
        options =>
            Uri.TryCreate(
                options.BaseUrl,
                UriKind.Absolute,
                out _
            ),
        "Sistemas360:BaseUrl debe ser una URL válida."
    )
    .Validate(
        options =>
            !string.IsNullOrWhiteSpace(
                options.Token
            ),
        "Falta Sistemas360:Token."
    )
    .ValidateOnStart();

builder.Services.AddHttpClient<Sistemas360Client>(
    (
        serviceProvider,
        httpClient
    ) =>
    {
        var options =
            serviceProvider
                .GetRequiredService<
                    IOptions<Sistemas360Options>
                >()
                .Value;

        httpClient.BaseAddress =
            new Uri(
                options.BaseUrl.TrimEnd('/') + "/"
            );

        httpClient.Timeout =
            TimeSpan.FromSeconds(30);

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                options.Token
            );

        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue(
                "application/json"
            )
        );
    }
);

builder.Services.AddScoped<
    ISistemas360Service,
    Sistemas360Service
>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
