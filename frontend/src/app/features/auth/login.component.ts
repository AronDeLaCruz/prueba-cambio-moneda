import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../core/auth.service';
import { ApiError } from '../../core/models';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  template: `
    <div class="min-h-screen flex items-center justify-center bg-slate-100 p-4">
      <div class="w-full max-w-md bg-white rounded-2xl shadow p-8">
        <h1 class="text-2xl font-bold text-slate-800">Cambio de Moneda</h1>
        <p class="text-sm text-slate-500 mt-1">Inicia sesión para continuar</p>
        <form [formGroup]="form" (ngSubmit)="onSubmit()" class="mt-6 space-y-4">
          <div>
            <label class="text-sm font-medium">Usuario</label>
            <input formControlName="usuario" class="mt-1 w-full border rounded-lg px-3 py-2"
              placeholder="admin" autocomplete="username" />
          </div>
          <div>
            <label class="text-sm font-medium">Password</label>
            <input formControlName="password" type="password" class="mt-1 w-full border rounded-lg px-3 py-2"
              placeholder="••••••" autocomplete="current-password" />
          </div>
          @if (error()) {
            <p class="text-sm text-red-600 bg-red-50 border border-red-200 rounded-lg px-3 py-2">{{ error() }}</p>
          }
          <button type="submit" [disabled]="form.invalid || loading()"
            class="w-full bg-blue-600 hover:bg-blue-700 disabled:opacity-50 text-white font-semibold rounded-lg py-2">
            {{ loading() ? 'Ingresando...' : 'Ingresar' }}
          </button>
          <p class="text-xs text-slate-400">Demo: admin / admin123 · API http://localhost:8080</p>
        </form>
      </div>
    </div>
  `,
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  loading = signal(false);
  error = signal<string | null>(null);

  form = this.fb.group({
    usuario: ['admin', Validators.required],
    password: ['admin123', Validators.required],
  });

  onSubmit(): void {
    if (this.form.invalid) return;
    this.loading.set(true);
    this.error.set(null);
    this.auth.login(this.form.getRawValue() as { usuario: string; password: string }).subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/conversion']);
      },
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);
        const body = err.error as ApiError | string | null;
        this.error.set(
          typeof body === 'string' ? body : body?.message ?? body?.mensaje ?? 'Credenciales inválidas'
        );
      },
    });
  }
}
