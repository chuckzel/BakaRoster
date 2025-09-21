using OneRosterProviderDemo.Models;

namespace OneRosterProviderDemo.Bakalari;

public class BakaSync
{
    private readonly BakaDb _bakaDb;
    private readonly ApiContext _apiDb;

    public BakaSync(BakaDb bakaDb, ApiContext apiDb)
    {
        _bakaDb = bakaDb;
        _apiDb = apiDb;
    }

    public void Sync()
    {
        
    }
    
}