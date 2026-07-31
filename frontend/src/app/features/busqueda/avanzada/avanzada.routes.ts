import { Routes } from '@angular/router';

export const AVANZADA_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/busqueda-avanzada-overview/busqueda-avanzada-overview.component').then(
        (m) => m.BusquedaAvanzadaOverviewComponent,
      ),
  },
];
