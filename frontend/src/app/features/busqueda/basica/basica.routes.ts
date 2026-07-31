import { Routes } from '@angular/router';

export const BASICA_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/busqueda-overview/busqueda-overview.component').then((m) => m.BusquedaOverviewComponent),
  },
];
