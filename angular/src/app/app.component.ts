import { Component, inject } from '@angular/core';
import { AuthService, DynamicLayoutComponent } from '@abp/ng.core';
import { LoaderBarComponent, NavItemsService } from '@abp/ng.theme.shared';
import { NotificationBellComponent } from './notifications/notification-bell.component';

@Component({
  selector: 'app-root',
  template: `
    <abp-loader-bar />
    <abp-dynamic-layout />
  `,
  imports: [LoaderBarComponent, DynamicLayoutComponent],
})
export class AppComponent {
  private authService = inject(AuthService);

  constructor(private navItems: NavItemsService) {
    this.navItems.addItems([
      {
        id: 'NotificationBell',
        component: NotificationBellComponent,
        order: 100,
        visible: () => this.authService.isAuthenticated,
      },
    ]);
  }
}
