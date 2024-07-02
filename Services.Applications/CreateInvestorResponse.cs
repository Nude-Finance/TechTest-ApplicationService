namespace Services.Applications;

public record CreateInvestorResponse(
    string Reference,
    string InvestorId,
    string AccountId,
    string PaymentId);