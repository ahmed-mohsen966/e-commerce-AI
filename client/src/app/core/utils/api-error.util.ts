import { HttpErrorResponse } from '@angular/common/http';

import { isValidationProblemDetails, ProblemDetails } from '../models/problem-details.model';

/** Extracts a single human-readable message from an RFC 7807 ProblemDetails error response. */
export function extractErrorMessage(error: unknown, fallback = 'Something went wrong. Please try again.'): string {
  if (!(error instanceof HttpErrorResponse)) {
    return fallback;
  }

  const problem = error.error as ProblemDetails | undefined;
  if (!problem) {
    return fallback;
  }

  if (isValidationProblemDetails(problem)) {
    const firstError = Object.values(problem.errors)[0]?.[0];
    if (firstError) {
      return firstError;
    }
  }

  return problem.detail || problem.title || fallback;
}
