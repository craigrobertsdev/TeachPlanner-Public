namespace TeachPlanner.Shared.Contracts.Authentication;

public record AuthenticationResponse(
    string FirstName,
    string LastName,
    string Token,
    DateTime Expiration,
    string RefreshToken,
    bool AccountSetupStatus);