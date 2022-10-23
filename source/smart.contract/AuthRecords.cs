using System;

namespace smart.contract;


public record AuthRequestDto(
    string Username,
    string Password);
public record AuthResponseDto(
    string Username,
    string JwtToken,
    string RefreshToken,
    DateTime Expiration);
public record RevokeTokenRequest(
    string Token);

public record RefreshTokenRequest(
    string Token);

public record ChangePasswordRequest(
    string Current,
    string New,
    string Repeated);
