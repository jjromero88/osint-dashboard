import { ChangeDetectionStrategy, Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { interval, switchMap, takeWhile } from 'rxjs';
import { NotificationService } from '../../../../../shared/services/notification.service';
import { Pais } from '../../../../../shared/models/pais.model';
import { PaisesService } from '../../../../../shared/services/paises.service';
import { BusquedaAvanzada } from '../../models/busqueda-avanzada.model';
import { BusquedaAvanzadaService } from '../../services/busqueda-avanzada.service';

const ESTADOS_EN_CURSO = ['queued', 'running'];

// Inputs de texto separados por coma en vez de un editor de listas
// dedicado — suficiente para el volumen de datos de este flujo (máx. 5
// valores por campo, ver validador de backend).
@Component({
  selector: 'osint-busqueda-avanzada-overview',
  imports: [FormsModule],
  templateUrl: './busqueda-avanzada-overview.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BusquedaAvanzadaOverviewComponent implements OnInit {
  private readonly _service = inject(BusquedaAvanzadaService);
  private readonly _paisesService = inject(PaisesService);
  private readonly _notif = inject(NotificationService);
  private readonly _destroyRef = inject(DestroyRef);

  loteActual = signal<BusquedaAvanzada | null>(null);
  historial = signal<BusquedaAvanzada[]>([]);
  buscando = signal(false);
  paises = signal<Pais[]>([]);

  usernames = '';
  emails = '';
  phones = '';
  paisTelefonos = '+51';
  domains = '';
  names = '';

  ngOnInit(): void {
    this.paises.set(this._paisesService.getAll());
    this._cargarHistorial();
  }

  buscar(): void {
    const dto = {
      usernames: this._aLista(this.usernames),
      emails: this._aLista(this.emails),
      // El mismo país aplica a todos los números de este campo — si el usuario ya
      // escribió un '+código' propio en una entrada puntual, se respeta tal cual.
      phones: this._aLista(this.phones).map((n) => (n.startsWith('+') ? n : `${this.paisTelefonos}${n}`)),
      domains: this._aLista(this.domains),
      names: this._aLista(this.names),
    };

    if (dto.usernames.length + dto.emails.length + dto.phones.length + dto.domains.length + dto.names.length === 0) {
      this._notif.error('Ingresa al menos un dato (username, email, teléfono, dominio o nombre).');
      return;
    }

    this.buscando.set(true);
    this.loteActual.set(null);
    this._service.iniciar(dto).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.loteActual.set(res.data);
          this._pollear(res.data.lote_id);
        } else {
          this._notif.error(res.message);
          this.buscando.set(false);
        }
      },
      error: () => {
        this._notif.error('No se pudo conectar con el servidor.');
        this.buscando.set(false);
      },
    });
  }

  private _pollear(loteId: string): void {
    interval(3000)
      .pipe(
        switchMap(() => this._service.getById(loteId)),
        takeWhile((res) => ESTADOS_EN_CURSO.includes(res.data?.estado ?? ''), true),
        takeUntilDestroyed(this._destroyRef),
      )
      .subscribe({
        next: (res) => {
          if (!res.success || !res.data) {
            this._notif.error(res.message);
            this.buscando.set(false);
            return;
          }
          this.loteActual.set(res.data);
          if (!ESTADOS_EN_CURSO.includes(res.data.estado)) {
            this.buscando.set(false);
            this._cargarHistorial();
          }
        },
        error: () => {
          this._notif.error('No se pudo conectar con el servidor.');
          this.buscando.set(false);
        },
      });
  }

  private _cargarHistorial(): void {
    this._service.getAll().subscribe({
      next: (res) => {
        if (res.success) this.historial.set(res.data ?? []);
      },
      error: () => this._notif.error('No se pudo cargar el historial.'),
    });
  }

  private _aLista(valor: string): string[] {
    return valor
      .split(',')
      .map((v) => v.trim())
      .filter((v) => v.length > 0);
  }

  protected _estadoTexto(estado: string): string {
    if (estado === 'failed') return 'error';
    if (ESTADOS_EN_CURSO.includes(estado)) return estado;
    return 'listo';
  }

  protected _estadoClase(estado: string): string {
    if (estado === 'failed') return 'border-accent-danger text-accent-danger';
    if (ESTADOS_EN_CURSO.includes(estado)) return 'border-accent-info text-accent-info animate-status-pulse';
    return 'border-accent-success text-accent-success';
  }

  protected _glow(estado: string): string {
    if (estado === 'failed') return 'var(--shadow-status-danger)';
    if (ESTADOS_EN_CURSO.includes(estado)) return 'var(--shadow-status-info)';
    return 'var(--shadow-status-success)';
  }
}
