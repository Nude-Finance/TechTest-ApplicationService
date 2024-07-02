using Services.Common.Abstractions.Model;

namespace Services.Applications;

public class AdministratorException(Error error, Exception? inner = null) 
    : Exception(error.Description, inner);