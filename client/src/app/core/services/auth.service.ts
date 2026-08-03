import { HttpClient } from '@angular/common/http';
import { Injectable, computed, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';

import { environment } from '../../../environments/environment';
import { AuthResponse, CurrentUser, LoginRequest, RegisterRequest, Role } from '../models/auth.models';
import { decodeJwtPayload } from '../utils/jwt.util';

const STORAGE_KEY = 'ecommerce.auth';

function loadStoredTokens(): AuthResponse | null {
  const raw = localStorage.getItem(STORAGE_KEY);
  if (!raw) {
    return null;
  }
  try {
    return JSON.parse(raw) as AuthResponse;
  } catch {
    return null;
  }
}

function toUser(tokens: AuthResponse | null): CurrentUser | null {
  if (!tokens) {
    return null;
  }
  const payload = decodeJwtPayload(tokens.accessToken);
  if (!payload) {
    return null;
  }

  const roleClaim = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
  const roles: Role[] = roleClaim ? (Array.isArray(roleClaim) ? roleClaim : [roleClaim]) : [];

  return {
    id: payload.sub,
    email: payload.email,
    customerId: payload.customerId ?? null,
    roles
  };
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly tokens = signal<AuthResponse | null>(loadStoredTokens());

  readonly currentUser = computed<CurrentUser | null>(() => toUser(this.tokens()));
  readonly isAuthenticated = computed(() => this.currentUser() !== null);

  constructor(private readonly http: HttpClient) {}

  get accessToken(): string | null {
    return this.tokens()?.accessToken ?? null;
  }

  get refreshToken(): string | null {
    return this.tokens()?.refreshToken ?? null;
  }

  hasRole(role: Role): boolean {
    return this.currentUser()?.roles.includes(role) ?? false;
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${environment.apiBaseUrl}/auth/login`, request)
      .pipe(tap((response) => this.setTokens(response)));
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${environment.apiBaseUrl}/auth/register`, request)
      .pipe(tap((response) => this.setTokens(response)));
  }

  refresh(): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${environment.apiBaseUrl}/auth/refresh-token`, {
        refreshToken: this.refreshToken
      })
      .pipe(tap((response) => this.setTokens(response)));
  }

  logout(): void {
    this.tokens.set(null);
    localStorage.removeItem(STORAGE_KEY);
  }

  private setTokens(response: AuthResponse): void {
    this.tokens.set(response);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(response));
  }
}
