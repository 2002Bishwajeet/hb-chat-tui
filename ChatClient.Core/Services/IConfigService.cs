namespace ChatClient.Core.Services;

public interface IConfigService
{
    string GetHomebaseId();
    void SetHomebaseId(string homebaseId);
}
