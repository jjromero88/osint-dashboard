import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { NotificationService, Notificacion } from '../../services/notification.service';

const TONOS: Record<Notificacion['tipo'], { surface: string; icon: string; glow: string }> = {
  exito: {
    surface: 'bg-[color-mix(in_srgb,var(--color-accent-success)_12%,var(--color-surface))]',
    icon: 'text-accent-success',
    glow: 'var(--shadow-status-success)',
  },
  error: {
    surface: 'bg-[color-mix(in_srgb,var(--color-accent-danger)_14%,var(--color-surface))]',
    icon: 'text-accent-danger',
    glow: 'var(--shadow-status-danger)',
  },
  info: {
    surface: 'bg-[color-mix(in_srgb,var(--color-accent-info)_12%,var(--color-surface))]',
    icon: 'text-accent-info',
    glow: 'var(--shadow-status-info)',
  },
};

@Component({
  selector: 'osint-notification-host',
  imports: [],
  templateUrl: './notification-host.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NotificationHostComponent {
  protected readonly _notif = inject(NotificationService);

  protected _tono(tipo: Notificacion['tipo']) {
    return TONOS[tipo];
  }
}
