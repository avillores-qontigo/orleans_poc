namespace OrleansURLShortener.Grains
{
    public interface IUrlShortenerGrain : IGrainWithStringKey
    {
        Task SetUrl(string longUrl);

        Task<string?> GetUrl();
    }
}
