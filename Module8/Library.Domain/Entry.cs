using System.Net;
using System.Text;
using Library.Domain.Abstractions.Services;
using Library.Domain.Services;
using Library.SharedKernel.Enums;
using Library.SharedKernel.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;

namespace Library.Domain;

public static class Entry
{
    public static IServiceCollection AddDomain(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddScoped<IBookService, BookService>();
        serviceCollection.AddSingleton<IReviewService, ReviewService>();
        serviceCollection.AddScoped<IAuthorService, AuthorService>();

        serviceCollection.AddHttpClient<IAuthorService, AuthorService>(c =>
        {
            c.BaseAddress = new Uri("https://api.coindesk.com/");
        })
        .AddTransientHttpErrorPolicy(p => 
            p.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1))
        )
        .AddTransientHttpErrorPolicy(p =>
            p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30))
        )
        .AddPolicyHandler(Policy<HttpResponseMessage>.Handle<BrokenCircuitException>()
            .FallbackAsync(
                fallbackAction: _ =>
                {
                    var errorResult = Result<string>.Failure(new Error(ErrorType.ServerError, 
                        "Не удалось подключиться..."));
                    var json = JsonConvert.SerializeObject(errorResult);
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json")
                    });
                })
        );

        return serviceCollection;
    }
}
