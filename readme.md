# Application Service

## Background

A fintech company distributes `ProductOne` using an external company, `AdministratorOne`, to manage the Investors, Accounts and Payments.

The company wants to launch `ProductTwo` using `AdministratorTwo`.  

In the future, the company might use `AdministratorTwo` for `ProductOne`.  And, of course, you can imagine the company will want to quickly launch more Products.

### Goal

The goal of this exercise is to process Applications for both Products, ensuring the appropriate Administrator is used to create an Investor and Account, and to process the initial Payment.

### Requirements
- Complete the implementation of `ApplicationProcessor`
  - Implement the `Process` method to process Applications for both Products.
- Only valid Applications should be processed
    - `ProductOne` is only available to people aged 18 to 39.
    - `ProductTwo` is only available to people aged 18 and above.
    - The minimum Payment for each Product is currently £0.99 but likely to change.
- New Users must be Kyc'd. Only Applications from verified Users should be processed.
- All outputs of the Application process must be captured by downstream systems which subscribe to `DomainEvents`. Some examples are included.

### Tips

- Write clean, readable code.
- Write behavioural tests.  You must get the first test in `ProductOneTests` to pass.  Add further similar tests for other scenarios for both products. 
  - Ensure `dotnet test` runs your tests and they all pass.
- Demonstrate knowledge of SOLID principles and design patterns.
- Be a perfectionist.  But, remember, done is better than perfect.
- Keep a simple log of your decisions etc.  See [log](log.md)
- Be prepared to discuss your solution in a follow-up interview.
- 

### Notes

- Both Administrators provide the same functionality but expose different interfaces.
- `Services.AdministratorOne.Abstractions` and `Services.AdministratorTwo.Abstractions` are third-party libraries.  You can't change them.
- You do not need to implement `IServiceAdministrator` for either Administrators.  Similarly, you don't need to implement `IKycService` or `IBus`. 
  - But you might need to stub or mock them in your tests.
- You may add new projects to the solution, and add appropriate nuget references.
- You may change and add new types defined in `Services.Common.Model`. But, because this library is shared within the company, you'll need to justify your decision.
- The exercise should take around two hours.  Don't implement:
    - Simple validation (required, min-length etc).
    - Argument checking
    - Logging
    - Exception handling
  
Good luck! When you're ready, push and let Tembo know when you're done.
