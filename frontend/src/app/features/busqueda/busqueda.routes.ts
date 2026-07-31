import { Routes } from '@angular/router';

export const BUSQUEDA_ROUTES: Routes = [
  { path: '', redirectTo: 'basica', pathMatch: 'full' },
  { path: 'basica', loadChildren: () => import('./basica/basica.routes').then((m) => m.BASICA_ROUTES) },
  { path: 'avanzada', loadChildren: () => import('./avanzada/avanzada.routes').then((m) => m.AVANZADA_ROUTES) },
];
