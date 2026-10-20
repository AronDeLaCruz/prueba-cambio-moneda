import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { API_URL, LoginRequest, LoginResponse } from './models';
import { TokenStorageService } from './token-storage.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private storage = inject(TokenStorageService);

  login(body: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${API_URL}/api/auth/login`, body).pipe(
      tap((res) => this.storage.save(res.token, res.expira, body.usuario))
    );
  }

  logout(): void {
    this.storage.clear();
  }

  isLoggedIn(): boolean {
    return this.storage.isLoggedIn();
  }

  getUsuario(): string | null {
    return this.storage.getUsuario();
  }

  getToken(): string | null {
    return this.storage.getToken();
  }
}
