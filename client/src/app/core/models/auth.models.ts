export type Role = 'Admin' | 'Customer';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface AuthResponse {
  accessToken: string;
  accessTokenExpiresAtUtc: string;
  refreshToken: string;
  refreshTokenExpiresAtUtc: string;
}

/** Decoded JWT access token payload — only the claims this app reads. */
export interface JwtPayload {
  sub: string;
  email: string;
  jti: string;
  customerId?: string;
  exp: number;
  /** .NET's ClaimTypes.Role resolves to this full URI, not the short "role". */
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'?: Role | Role[];
}

export interface CurrentUser {
  id: string;
  email: string;
  customerId: string | null;
  roles: Role[];
}
