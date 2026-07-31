import { Injectable, inject } from '@angular/core';
import { Observable, map, shareReplay } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { Herramienta } from '../models/herramienta.model';

// Catálogo con cache — lo usan tanto la búsqueda básica como la avanzada,
// por eso vive a la altura del feature 'busqueda' y no dentro de un solo modo.
@Injectable({ providedIn: 'root' })
export class HerramientasService {
  private readonly _api = inject(ApiService);
  private _cache$: Observable<Herramienta[]> | null = null;

  getAll(): Observable<Herramienta[]> {
    if (!this._cache$) {
      this._cache$ = this._api.get<Herramienta[]>('/tools').pipe(
        map((res) => res.data ?? []),
        shareReplay(1),
      );
    }
    return this._cache$;
  }
}
