using Odin.Core.Identity;
using Odin.Services.Configuration;
using Odin.Services.Configuration.Eula;
using Refit;

namespace Odin.ClientApi.Owner.ApiClient.Configuration;

public class OwnerConfigurationApiClient(OdinId identity, IApiClientFactory factory)
{
    private IRefitOwnerConfiguration CreateRefitClient()
    {
        return factory.CreateRefitHttpClient<IRefitOwnerConfiguration>(identity);
    }

    public async Task<ApiResponse<bool>> InitializeIdentity(InitialSetupRequest setupConfig)
    {
        var svc = this.CreateRefitClient();
        return await svc.InitializeIdentity(setupConfig);
    }

    public async Task<ApiResponse<bool>> IsIdentityConfigured()
    {
        var svc = this.CreateRefitClient();
        return await svc.IsIdentityConfigured();
    }

    public async Task<ApiResponse<HttpContent>> MarkEulaSigned(MarkEulaSignedRequest request)
    {
        var svc = this.CreateRefitClient();
        return await svc.MarkEulaSigned(request);
    }

    public async Task<ApiResponse<bool>> IsEulaSignatureRequired()
    {
        var svc = this.CreateRefitClient();
        return await svc.IsEulaSignatureRequired();
    }

    public async Task<ApiResponse<bool>> UpdateTenantSettingsFlag(TenantConfigFlagNames flag, string value)
    {
        var svc = this.CreateRefitClient();

        var updateFlagResponse = await svc.UpdateSystemConfigFlag(new UpdateFlagRequest()
        {
            FlagName = Enum.GetName(flag),
            Value = value
        });

        return updateFlagResponse;
    }

    public async Task<ApiResponse<TenantSettings>> GetTenantSettings()
    {
        var svc = this.CreateRefitClient();
        return await svc.GetTenantSettings();
    }

    public async Task<ApiResponse<OwnerAppSettings>> GetOwnerAppSettings()
    {
        var svc = this.CreateRefitClient();
        return await svc.GetOwnerAppSettings();
    }

    public async Task<ApiResponse<bool>> UpdateOwnerAppSetting(OwnerAppSettings ownerSettings)
    {
        var svc = this.CreateRefitClient();
        return await svc.UpdateOwnerAppSetting(ownerSettings);
    }

    public async Task<ApiResponse<List<EulaSignature>>> GetEulaSignatureHistory()
    {
        var svc = this.CreateRefitClient();
        return await svc.GetEulaSignatureHistory();
    }

    public async Task DisableAutoAcceptIntroductions(bool disabled)
    {
        var updateTenantSettingsFlagResponse = await this.UpdateTenantSettingsFlag(
            TenantConfigFlagNames.DisableAutoAcceptIntroductionsForTests, disabled.ToString());

        if (!updateTenantSettingsFlagResponse.IsSuccessStatusCode)
        {
            throw new Exception("test setup failed");
        }
    }
    
}