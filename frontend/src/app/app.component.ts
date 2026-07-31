import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NotificationHostComponent } from './shared/ui/notification-host/notification-host.component';

@Component({
  selector: 'osint-root',
  imports: [RouterOutlet, NotificationHostComponent],
  templateUrl: './app.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppComponent {}
