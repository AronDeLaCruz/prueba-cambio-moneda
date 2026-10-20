import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../core/auth.service';

@Component({
  selector: 'app-layout',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <nav class="bg-slate-900 text-white px-4 py-3 flex items-center gap-4">
      <span class="font-bold">💱 CambioMoneda</span>
      <a routerLink="/conversion" routerLinkActive="underline" class="text-sm hover:underline">Conversión</a>
      <a routerLink="/tipos-cambio" routerLinkActive="underline" class="text-sm hover:underline">Tipos de cambio</a>
      <span class="ml-auto text-sm text-slate-300">{{ auth.getUsuario() }}</span>
      <button (click)="salir()" class="text-sm bg-slate-700 hover:bg-slate-600 rounded-lg px-3 py-1">Salir</button>
    </nav>
    <main class="max-w-5xl mx-auto p-4">
      <router-outlet />
    </main>
  `,
})
export class LayoutComponent {
  auth = inject(AuthService);
  private router = inject(Router);

  salir(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
