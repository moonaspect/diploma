﻿using System.Net.Http;
using System.Text.Json;
using Japanese.ViewModels;

public class WordPairService : IWordPairService
{
    private static readonly HttpClient HttpClient = new();
    private readonly string _apiUrlBase =
        "https://lkrfzpjnh7.execute-api.eu-north-1.amazonaws.com/prod/words?pageSize=100&pageNumber=1";

    private readonly ICache<WordPair> _cachedWordPairs;

    public WordPairService(ICache<WordPair> cachingService)
    {
        _cachedWordPairs = cachingService;
    }

    public WordPairService(string apiUrlBase, ICache<WordPair> cachingService)
        : this(cachingService)
    {
        _apiUrlBase = apiUrlBase;
    }

    public IList<WordPair> GetWordPairs()
    {
        return _cachedWordPairs.GetCachedDataAsync(LoadDataAsync).GetAwaiter().GetResult();
    }

    private async Task<IList<WordPair>> LoadDataAsync()
    {
        try
        {
            var response = await HttpClient.GetAsync(_apiUrlBase).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var wordPairsResponse = JsonSerializer.Deserialize<WordPairResponse>(responseBody);

            return wordPairsResponse?.Items ?? new List<WordPair>();
        }
        catch (Exception)
        {
            return new List<WordPair>();
        }
    }
}
