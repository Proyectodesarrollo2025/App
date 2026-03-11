import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService, AppNotificationDto } from './notification.service';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';

@Component({
    selector: 'app-notification-bell',
    standalone: true,
    imports: [CommonModule, CoreModule, ThemeSharedModule],
    template: `
    <div class="dropdown" ngbDropdown placement="bottom-right">
      <a
        class="nav-link dropdown-toggle"
        data-toggle="dropdown"
        ngbDropdownToggle
        role="button"
        aria-expanded="false"
        (click)="toggleDropdown()"
      >
        <i class="fa fa-bell"></i>
        <span class="badge bg-danger position-absolute top-0 start-100 translate-middle badge-pill" *ngIf="unreadCount > 0">
          {{ unreadCount }}
        </span>
      </a>

      <div class="dropdown-menu dropdown-menu-right" ngbDropdownMenu style="width: 300px; max-height: 400px; overflow-y: auto;">
        <h6 class="dropdown-header">Notificaciones</h6>
        <div class="dropdown-divider"></div>
        <ng-container *ngIf="notifications.length > 0; else noNotifs">
          <button class="dropdown-item" *ngFor="let n of notifications" (click)="markAsRead(n)" [class.fw-bold]="!n.isRead">
            <div class="d-flex w-100 justify-content-between">
              <h6 class="mb-1">{{ n.title }}</h6>
            </div>
            <p class="mb-1 text-wrap" style="font-size: 0.85em;">{{ n.message }}</p>
            <small>{{ n.creationTime | date:'short' }}</small>
          </button>
        </ng-container>
        <ng-template #noNotifs>
          <div class="p-3 text-center text-muted">No tienes notificaciones.</div>
        </ng-template>
        <div class="dropdown-divider"></div>
        <button class="dropdown-item text-center text-primary" (click)="markAllAsRead()" *ngIf="unreadCount > 0">
          Marcar todas como leídas
        </button>
      </div>
    </div>
  `,
    styles: [`
    .dropdown-toggle::after { display: none; }
    .nav-link { position: relative; cursor: pointer; }
    .text-wrap { white-space: normal; }
  `]
})
export class NotificationBellComponent implements OnInit {
    notifications: AppNotificationDto[] = [];
    unreadCount = 0;

    constructor(private notificationService: NotificationService) { }

    ngOnInit() {
        this.loadNotifications();
        this.loadUnreadCount();
    }

    loadNotifications() {
        this.notificationService.getNotifications().subscribe((res) => {
            this.notifications = res.items;
        });
    }

    loadUnreadCount() {
        this.notificationService.getUnreadCount().subscribe((count) => {
            this.unreadCount = count;
        });
    }

    toggleDropdown() {
        this.loadNotifications();
    }

    markAsRead(notification: AppNotificationDto) {
        if (!notification.isRead) {
            this.notificationService.markAsRead(notification.id).subscribe(() => {
                notification.isRead = true;
                this.unreadCount = Math.max(0, this.unreadCount - 1);
            });
        }
    }

    markAllAsRead() {
        this.notificationService.markAllAsRead().subscribe(() => {
            this.notifications.forEach(n => n.isRead = true);
            this.unreadCount = 0;
        });
    }
}
