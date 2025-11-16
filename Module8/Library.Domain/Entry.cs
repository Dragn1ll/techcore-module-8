using Library.Domain.Abstractions.Services;
using Library.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;

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
        );

        return serviceCollection;
    }
}
