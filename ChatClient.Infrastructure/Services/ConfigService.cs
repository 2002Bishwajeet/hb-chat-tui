using ChatClient.Core.Services;

namespace ChatClient.Infrastructure.Services;

public class ConfigService : IConfigService
{
    private string _homebaseId = string.Empty;

    public string GetHomebaseId()
    {
        return _homebaseId;
    }

    public void SetHomebaseId(string homebaseId)
    {
        _homebaseId = homebaseId;
    }
}
