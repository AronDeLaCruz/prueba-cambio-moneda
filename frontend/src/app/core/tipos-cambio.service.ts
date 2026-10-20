import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL, TipoCambio, TipoCambioCreate } from './models';

@Injectable({ providedIn: 'root' })
export class TiposCambioService {
  private http = inject(HttpClient);
  private base = `${API_URL}/api/tipos-cambio`;

  listar(): Observable<TipoCambio[]> {
    return this.http.get<TipoCambio[]>(this.base);
  }

  crear(body: TipoCambioCreate): Observable<TipoCambio> {
    return this.http.post<TipoCambio>(this.base, body);
  }

  actualizar(id: number, body: TipoCambioCreate): Observable<TipoCambio> {
    return this.http.put<TipoCambio>(`${this.base}/${id}`, body);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
