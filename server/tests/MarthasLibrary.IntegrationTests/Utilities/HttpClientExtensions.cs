using System.Net.Http.Formatting;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RaphaIdServer.Tests.Common.Helpers;

public static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions JsonSerializerOptions
      = new(JsonSerializerDefaults.Web);

    static HttpClientExtensions()
    {
        // since the API serializes enums,
        // we need to let HttpClient know to use those settings.
        // Unfortunately, there's no way to do that globally, so we have to create extension methods.
        JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }

    /// <summary>
    /// Does GetFromJsonAsync, but with custom json serializer options.
    /// </summary>
    /// <param name="client">HttpClient.</param>
    /// <param name="url">The URL to access.</param>
    /// <typeparam name="T">ApiResponse body type.</typeparam>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static Task<T?> GetFromJsonFixedAsync<T>(this HttpClient client, string url)
    {
        return client.GetFromJsonAsync<T>(url, JsonSerializerOptions);
    }

    /// <summary>
    /// <see cref="GetFromJsonFixedAsync{T}(HttpClient,string)"/>.
    /// </summary>
    /// <param name="client">HttpClient.</param>
    /// <param name="url">The URL to access.</param>
    /// <typeparam name="T">ApiResponse body type.</typeparam>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static Task<T?> GetFromJsonFixedAsync<T>(this HttpClient client, Uri url)
    {
        return client.GetFromJsonAsync<T>(url, JsonSerializerOptions);
    }

    /// <summary>
    /// Does ReadFromJsonAsync, but with custom json serializer options.
    /// </summary>
    /// <param name="content">HttpContent.</param>
    /// <typeparam name="T">ApiResponse body type.</typeparam>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static Task<T?> ReadFromJsonFixedAsync<T>(this HttpContent content)
    {
        return content.ReadFromJsonAsync<T>(JsonSerializerOptions);
    }

    public static Task<HttpResponseMessage> PatchAsJsonAsync<T>(this HttpClient client, string requestUri, T value)
    {
        var content = new ObjectContent<T>(value, new JsonMediaTypeFormatter());
        var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri) { Content = content };

        return client.SendAsync(request);
    }
}
