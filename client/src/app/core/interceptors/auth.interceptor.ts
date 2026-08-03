import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

import { AuthService } from '../services/auth.service';

/** Endpoints that don't require (and shouldn't receive) a bearer token. */
const PUBLIC_PATHS = ['/auth/login', '/auth/register', '/auth/refresh-token'];

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const isPublic = PUBLIC_PATHS.some((path) => req.url.includes(path));
  const token = authService.accessToken;

  const authedReq = !isPublic && token ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : req;

  return next(authedReq).pipe(
    catchError((error: unknown) => {
      // Only treat this as "session expired" if we actually sent a bearer token that got
      // rejected. A 401 on a request that never carried a token (an anonymous visitor hitting
      // an endpoint that requires auth, e.g. the categories filter on a public products page)
      // isn't an expired session — there was never one to expire — so don't log out or redirect.
      if (error instanceof HttpErrorResponse && error.status === 401 && !isPublic && token) {
        authService.logout();
        router.navigate(['/auth/login'], { queryParams: { returnUrl: router.url } });
      }
      return throwError(() => error);
    })
  );
};
