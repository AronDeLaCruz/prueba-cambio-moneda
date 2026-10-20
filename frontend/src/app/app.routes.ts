import { Routes } from '@angular/router';
import { authGuard } from './core/auth.guard';
import { LayoutComponent } from './shared/layout.component';
import { LoginComponent } from './features/auth/login.component';
import { ConversionComponent } from './features/conversion/conversion.component';
import { TiposCambioComponent } from './features/tipos-cambio/tipos-cambio.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'conversion', pathMatch: 'full' },
      { path: 'conversion', component: ConversionComponent },
      { path: 'tipos-cambio', component: TiposCambioComponent },
    ],
  },
  { path: '**', redirectTo: '' },
];
