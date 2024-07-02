using System.Collections.Concurrent;
using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;

namespace Services.Applications;

/// <summary>
/// Caches KYC reports so that <see cref="IKycService.GetKycReportAsync"/>
/// is only called once per user.
/// </summary>
/// <param name="inner"></param>
public class KycReportCache(IKycService inner) : IKycService
{
    private readonly ConcurrentDictionary<User, Task<Result<KycReport>>> _reports = new();
    
    public async Task<Result<KycReport>> GetKycReportAsync(User user)
    {
        return await _reports.GetOrAdd(user, inner.GetKycReportAsync);
    }
}