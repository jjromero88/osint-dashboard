import { ChangeDetectionStrategy, Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { interval, switchMap, takeWhile } from 'rxjs';
import { NotificationService } from '../../../../../shared/services/notification.service';
import { Pais } from '../../../../../shared/models/pais.model';
import { PaisesService } from '../../../../../shared/services/paises.service';
import { Herramienta } from '../../../models/herramienta.model';
import { HerramientasService } from '../../../services/herramientas.service';
import { Busqueda } from '../../models/busqueda.model';
import { BusquedaService } from '../../services/busqueda.service';

const ESTADOS_EN_CURSO = ['queued', 'running'];

@Component({
  selector: 'osint-busqueda-overview',
  imports: [FormsModule],
  templateUrl: './busqueda-overview.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BusquedaOverviewComponent implements OnInit {
  private readonly _herramientasService = inject(HerramientasService);
  private readonly _paisesService = inject(PaisesService);
  private readonly _busquedaService = inject(BusquedaService);
  private readonly _notif = inject(NotificationService);
  private readonly _destroyRef = inject(DestroyRef);

  tipos = signal<Herramienta[]>([]);
  paises = signal<Pais[]>([]);
  busquedaActual = signal<Busqueda | null>(null);
  buscando = signal(false);

  tipoSeleccionado = '';
  objetivo = '';
  paisSeleccionado = '+51';
  numeroLocal = '';

  ngOnInit(): void {
    this._herramientasService.getAll().subscribe({
      next: (tipos) => {
        this.tipos.set(tipos);
        if (tipos.length > 0) this.tipoSeleccionado = tipos[0].tipo;
      },
      error: () => this._notif.error('No se pudo cargar el catálogo de tipos de búsqueda.'),
    });
    this.paises.set(this._paisesService.getAll());
  }

  get esTelefono(): boolean {
    return this.tipoSeleccionado === 'phone';
  }

  buscar(): void {
    const objetivoFinal = this.esTelefono
      ? `${this.paisSeleccionado}${this.numeroLocal.trim()}`
      : this.objetivo.trim();

    if (!this.tipoSeleccionado || (this.esTelefono ? !this.numeroLocal.trim() : !objetivoFinal)) {
      this._notif.error(this.esTelefono ? 'Ingresa el número local.' : 'Elige un tipo e ingresa un objetivo.');
      return;
    }

    this.buscando.set(true);
    this.busquedaActual.set(null);
    this._busquedaService.iniciar({ tipo: this.tipoSeleccionado, objetivo: objetivoFinal }).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.busquedaActual.set(res.data);
          this._pollear(res.data.busqueda_id);
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

  private _pollear(busquedaId: string): void {
    interval(2000)
      .pipe(
        switchMap(() => this._busquedaService.getById(busquedaId)),
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
          this.busquedaActual.set(res.data);
          if (!ESTADOS_EN_CURSO.includes(res.data.estado)) {
            this.buscando.set(false);
          }
        },
        error: () => {
          this._notif.error('No se pudo conectar con el servidor.');
          this.buscando.set(false);
        },
      });
  }

  protected _estadoTexto(estado: string, error: string | null): string {
    if (error || estado === 'failed') return 'error';
    if (ESTADOS_EN_CURSO.includes(estado)) return estado;
    return 'listo';
  }

  protected _estadoClase(estado: string, error: string | null): string {
    if (error || estado === 'failed') return 'border-accent-danger text-accent-danger';
    if (ESTADOS_EN_CURSO.includes(estado)) return 'border-accent-info text-accent-info animate-status-pulse';
    return 'border-accent-success text-accent-success';
  }

  protected _glow(estado: string, error: string | null): string {
    if (error || estado === 'failed') return 'var(--shadow-status-danger)';
    if (ESTADOS_EN_CURSO.includes(estado)) return 'var(--shadow-status-info)';
    return 'var(--shadow-status-success)';
  }
}
