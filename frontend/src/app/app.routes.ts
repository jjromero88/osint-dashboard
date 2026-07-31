import { Routes } from '@angular/router';
import { ShellComponent } from './core/layout/shell/shell.component';

export const routes: Routes = [
  {
    path: '',
    component: ShellComponent,
    children: [
      { path: '', redirectTo: 'busqueda', pathMatch: 'full' },
      {
        path: 'busqueda',
        loadChildren: () => import('./features/busqueda/busqueda.routes').then((m) => m.BUSQUEDA_ROUTES),
      },
    ],
  },
];
