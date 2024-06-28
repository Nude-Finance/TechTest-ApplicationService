using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;

namespace Services.Applications;

public class ApplicationProcessor(IBus bus) : IApplicationProcessor
{
    public async Task Process(Application application)
    {
        int applicantAge = GetFullYearsSince(application.Applicant.DateOfBirth);
        
        DomainEvent result = applicantAge is >= 18 and <= 39 
            ? new ApplicationCompleted(application.Id)
            : new ApplicationFailed(application.Id);
        
        await bus.PublishAsync(result);
    }

    private static int GetFullYearsSince(DateOnly date)
    {
        DateTime today = DateTime.Now.Date;
        int age = today.Year - date.Year;

        if (date.Month < today.Month || (date.Month == today.Month && date.Day < today.Day))
        {
            --age;
        }

        return age;
    }
}