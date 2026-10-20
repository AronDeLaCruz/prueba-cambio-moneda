import { Injectable } from '@angular/core';

const TOKEN_KEY = 'cm_token';
const EXPIRA_KEY = 'cm_expira';
const USER_KEY = 'cm_user';

@Injectable({ providedIn: 'root' })
export class TokenStorageService {
  save(token: string, expira: string, usuario: string): void {
    localStorage.setItem(TOKEN_KEY, token);
    localStorage.setItem(EXPIRA_KEY, expira);
    localStorage.setItem(USER_KEY, usuario);
  }

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  getUsuario(): string | null {
    return localStorage.getItem(USER_KEY);
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    const expira = localStorage.getItem(EXPIRA_KEY);
    if (!token || !expira) return false;
    if (new Date(expira).getTime() <= Date.now()) {
      this.clear();
      return false;
    }
    return true;
    //return !!this.getToken();
  }

  clear(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(EXPIRA_KEY);
    localStorage.removeItem(USER_KEY);
  }
}
