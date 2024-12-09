using System.IO;
using System.Text.Json;

public class PersistentCache<T> : ICache<T>
{
    private readonly string? _filePath;
    private IList<T>? _cachedData;

    public PersistentCache(string? filePath = null)
    {
        _filePath = filePath;
    }

    public async Task<IList<T>> GetCachedDataAsync(Func<Task<IList<T>>> dataLoader)
    {
        try
        {
            // Return in-memory cache if available
            if (_cachedData != null)
            {
                return _cachedData;
            }

            // Try loading from file if enabled
            if (_filePath != null && File.Exists(_filePath) && new FileInfo(_filePath).Length > 0)
            {
                var fileContent = await File.ReadAllTextAsync(_filePath).ConfigureAwait(false);
                _cachedData = JsonSerializer.Deserialize<List<T>>(fileContent) ?? new List<T>();
                return _cachedData;
            }

            // Load data using the provided loader function and cache it
            _cachedData = await dataLoader().ConfigureAwait(false);

            // Save to file if enabled
            if (_filePath != null)
            {
                await SaveDataAsync(_cachedData);
            }

            return _cachedData;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
            return new List<T>();
        }
    }

    public async Task SaveDataAsync(IList<T> data)
    {
        _cachedData = data;

        if (_filePath != null)
        {
            var serializedData = JsonSerializer.Serialize(
                data,
                new JsonSerializerOptions { WriteIndented = true }
            );
            await File.WriteAllTextAsync(_filePath, serializedData);
        }
    }
}
