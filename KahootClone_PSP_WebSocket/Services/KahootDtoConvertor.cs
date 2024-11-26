using Newtonsoft.Json;

namespace KahootClone_PSP_WebSocket.Services;

public static class KahootDtoConvertor
{
    public static T ConvertToDto<T>(string jsonData)
    {
        if (string.IsNullOrEmpty(jsonData))
        {
            throw new ArgumentException("Input JSON data cannot be null or empty", nameof(jsonData));
        }

        try
        {
            return JsonConvert.DeserializeObject<T>(jsonData);
        }
        catch (JsonException ex)
        {
            // Выбросить исключение в случае ошибки десериализации
            throw new InvalidOperationException($"Failed to convert JSON to {typeof(T).Name}", ex);
        }
    }
}
