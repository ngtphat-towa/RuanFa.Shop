using Carter;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Net.Http.Headers;

namespace RuanFa.Shop.Web.Api.Extensions;

/// <summary>
/// A Newtonsoft implementation of <see cref="IResponseNegotiator"/>
/// </summary>
public class SnakeCaseNewtonsoftJsonResponseNegotiator : IResponseNegotiator
{
    private readonly JsonSerializerSettings jsonSettings;

    public SnakeCaseNewtonsoftJsonResponseNegotiator()
    {
        var contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        };
        jsonSettings = new JsonSerializerSettings { ContractResolver = contractResolver, NullValueHandling = NullValueHandling.Ignore };
    }
    public bool CanHandle(MediaTypeHeaderValue accept)
    {
        return accept.MediaType.ToString().IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0;
    }
    public Task Handle<T>(HttpRequest req, HttpResponse res, T model, CancellationToken cancellationToken)
    {
        res.ContentType = "application/json; charset=utf-8";
        return res.WriteAsync(JsonConvert.SerializeObject(model, jsonSettings), cancellationToken);
    }
}
