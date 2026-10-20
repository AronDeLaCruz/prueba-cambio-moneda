import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL, ConversionRequest, ConversionResponse } from './models';

@Injectable({ providedIn: 'root' })
export class ConversionService {
  private http = inject(HttpClient);

  convertir(body: ConversionRequest): Observable<ConversionResponse> {
    return this.http.post<ConversionResponse>(`${API_URL}/api/conversion`, body);
  }
}
