import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';

// Wrapper único sobre HttpClient — ningún componente/servicio de feature
// inyecta HttpClient directo, siempre pasan por acá.
@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly _http = inject(HttpClient);
  private readonly _baseUrl = environment.apiUrl;

  get<T>(path: string): Observable<ApiResponse<T>> {
    return this._http
      .get<ApiResponse<T>>(`${this._baseUrl}${path}`)
      .pipe(catchError((e) => this._handleError<T>(e)));
  }

  post<T>(path: string, body: unknown): Observable<ApiResponse<T>> {
    return this._http
      .post<ApiResponse<T>>(`${this._baseUrl}${path}`, body)
      .pipe(catchError((e) => this._handleError<T>(e)));
  }

  put<T>(path: string, body: unknown): Observable<ApiResponse<T>> {
    return this._http
      .put<ApiResponse<T>>(`${this._baseUrl}${path}`, body)
      .pipe(catchError((e) => this._handleError<T>(e)));
  }

  delete<T>(path: string): Observable<ApiResponse<T>> {
    return this._http
      .delete<ApiResponse<T>>(`${this._baseUrl}${path}`)
      .pipe(catchError((e) => this._handleError<T>(e)));
  }

  // Genérico en T (no unknown): así el observable de error sigue siendo
  // asignable al Observable<ApiResponse<T>> del método que lo llama, bajo
  // TypeScript strict — el ejemplo del skill no es genérico acá y no
  // compila en strict mode.
  private _handleError<T>(err: HttpErrorResponse): Observable<ApiResponse<T>> {
    if (err.error?.success !== undefined) return throwError(() => err.error as ApiResponse<T>);
    const message = err.status === 0 ? 'No se pudo conectar con el servidor.' : `Error inesperado (${err.status}).`;
    return throwError(() => ({ success: false, message, data: null, errors: null }) as ApiResponse<T>);
  }
}
