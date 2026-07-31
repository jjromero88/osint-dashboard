import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../../core/services/api.service';
import { ApiResponse } from '../../../../core/models/api-response.model';
import { BusquedaAvanzada, BusquedaAvanzadaRequestDto } from '../models/busqueda-avanzada.model';

@Injectable({ providedIn: 'root' })
export class BusquedaAvanzadaService {
  private readonly _api = inject(ApiService);

  iniciar(dto: BusquedaAvanzadaRequestDto): Observable<ApiResponse<BusquedaAvanzada>> {
    return this._api.post<BusquedaAvanzada>('/search/advanced', dto);
  }

  getById(loteId: string): Observable<ApiResponse<BusquedaAvanzada>> {
    return this._api.get<BusquedaAvanzada>(`/search/advanced/${loteId}`);
  }
}
