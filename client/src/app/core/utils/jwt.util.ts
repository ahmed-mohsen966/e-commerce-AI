import { JwtPayload } from '../models/auth.models';

/**
 * Decodes a JWT's payload without verifying the signature — verification is the
 * API's job. Base64url (not plain base64) requires swapping -/_ back to +/ before
 * atob, and re-padding to a multiple of 4 chars.
 */
export function decodeJwtPayload(token: string): JwtPayload | null {
  const segments = token.split('.');
  if (segments.length !== 3) {
    return null;
  }

  try {
    const base64 = segments[1].replace(/-/g, '+').replace(/_/g, '/');
    const padded = base64.padEnd(base64.length + ((4 - (base64.length % 4)) % 4), '=');
    const json = decodeURIComponent(
      atob(padded)
        .split('')
        .map((c) => '%' + c.charCodeAt(0).toString(16).padStart(2, '0'))
        .join('')
    );
    return JSON.parse(json) as JwtPayload;
  } catch {
    return null;
  }
}

export function isTokenExpired(payload: JwtPayload, skewSeconds = 10): boolean {
  return Date.now() / 1000 >= payload.exp - skewSeconds;
}
