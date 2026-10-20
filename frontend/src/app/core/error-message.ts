import { HttpErrorResponse } from '@angular/common/http';
import { ApiError } from './models';

export function getErrorMessage(err: HttpErrorResponse): string {
  if (err.status === 0) return 'No se pudo conectar con el servidor.';
  const e = err.error as (ApiError & { errors?: Record<string, string[]>; title?: string }) | null;
  if (e?.message) return e.message;
  if (e?.errors) return Object.values(e.errors).flat().join(' ');
  return e?.title ?? 'Ocurrió un error inesperado.';
}