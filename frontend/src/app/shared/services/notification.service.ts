import { Injectable, signal } from '@angular/core';

export interface Notificacion {
  tipo: 'exito' | 'error' | 'info';
  texto: string;
}

// Host de notificaciones (singleton). Sin diseño todavía — ver skill
// angular-design-system cuando se ataque el estilo visual real.
@Injectable({ providedIn: 'root' })
export class NotificationService {
  mensaje = signal<Notificacion | null>(null);

  exito(texto: string): void {
    this.mensaje.set({ tipo: 'exito', texto });
  }

  error(texto: string): void {
    this.mensaje.set({ tipo: 'error', texto });
  }

  info(texto: string): void {
    this.mensaje.set({ tipo: 'info', texto });
  }

  limpiar(): void {
    this.mensaje.set(null);
  }
}
