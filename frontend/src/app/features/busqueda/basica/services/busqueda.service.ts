import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../../core/services/api.service';
import { ApiResponse } from '../../../../core/models/api-response.model';
import { Busqueda, BusquedaRequestDto } from '../models/busqueda.model';

@Injectable({ providedIn: 'root' })
export class BusquedaService {
  private readonly _api = inject(ApiService);

  iniciar(dto: BusquedaRequestDto): Observable<ApiResponse<Busqueda>> {
    return this._api.post<Busqueda>('/search', dto);
  }

  getById(id: string): Observable<ApiResponse<Busqueda>> {
    return this._api.get<Busqueda>(`/search/${id}`);
  }

  getAll(): Observable<ApiResponse<Busqueda[]>> {
    return this._api.get<Busqueda[]>('/search');
  }
}
