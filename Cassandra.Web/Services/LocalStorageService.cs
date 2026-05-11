using Microsoft.JSInterop;

namespace Cassandra.Web.Services;

public class LocalStorageService(IJSRuntime js)
{
    public async Task<string?> GetItemAsync(string key)
    {
        try
        {
            return await js.InvokeAsync<string?>("localStorage.getItem", key);
        }
        catch
        {
            return null;
        }
    }

    public async Task SetItemAsync(string key, string value)
    {
        try
        {
            await js.InvokeVoidAsync("localStorage.setItem", key, value);
        }
        catch { }
    }

    public async Task RemoveItemAsync(string key)
    {
        try
        {
            await js.InvokeVoidAsync("localStorage.removeItem", key);
        }
        catch { }
    }
}
