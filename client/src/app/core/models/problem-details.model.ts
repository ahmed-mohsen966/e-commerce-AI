export interface ProblemDetails {
  type?: string;
  title: string;
  status: number;
  detail?: string;
  instance?: string;
}

export interface ValidationProblemDetails extends ProblemDetails {
  errors: Record<string, string[]>;
}

export function isValidationProblemDetails(
  problem: ProblemDetails | null | undefined
): problem is ValidationProblemDetails {
  return !!problem && typeof (problem as ValidationProblemDetails).errors === 'object';
}
