export const API_URL = 'http://localhost:8080';

export interface LoginRequest {
  usuario: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expira: string;
}

export interface TipoCambio {
  id: number;
  monedaOrigen: string;
  monedaDestino: string;
  tasa: number;
  fechaActualizacion: string;
}

export interface TipoCambioCreate {
  monedaOrigen: string;
  monedaDestino: string;
  tasa: number;
}

export interface ConversionRequest {
  monto: number;
  monedaOrigen: string;
  monedaDestino: string;
}

export interface ConversionResponse {
  monto: number;
  montoConTipoCambio: number;
  monedaOrigen: string;
  monedaDestino: string;
  tipoCambio: number;
}

export interface ApiError {
  message?: string;
  mensaje?: string;
  error?: string;
}
