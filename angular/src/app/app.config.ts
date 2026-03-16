import { provideAbpCore, withOptions } from '@abp/ng.core';
import { provideAbpOAuth } from '@abp/ng.oauth';
import { provideSettingManagementConfig } from '@abp/ng.setting-management/config';
import { provideFeatureManagementConfig } from '@abp/ng.feature-management';
import { provideAbpThemeShared } from '@abp/ng.theme.shared';
import { IDENTITY_ENTITY_ACTION_CONTRIBUTORS, IDENTITY_ENTITY_PROP_CONTRIBUTORS } from '@abp/ng.identity';
import { provideIdentityConfig } from '@abp/ng.identity/config';
import { provideAccountConfig } from '@abp/ng.account/config';
import { registerLocale } from '@abp/ng.core/locale';
import { provideThemeLeptonX } from '@abp/ng.theme.lepton-x';
import { provideSideMenuLayout } from '@abp/ng.theme.lepton-x/layouts';
import { provideLogo, withEnvironmentOptions } from "@volo/ngx-lepton-x.core";
import { ApplicationConfig } from '@angular/core';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideRouter } from '@angular/router';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { environment } from '../environments/environment';
import { APP_ROUTES } from './app.routes';
import { APP_ROUTE_PROVIDER } from './route.provider';
import { NotificationInterceptor } from './notifications/notification.interceptor';
import { 
  PUBLIC_PROFILE_ENTITY_ACTION_CONTRIBUTORS, 
  PUBLIC_PROFILE_ENTITY_PROP_CONTRIBUTORS 
} from './public-profile/public-profile-ui-extensions';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(APP_ROUTES),
    APP_ROUTE_PROVIDER,
    provideAnimations(),
    provideAbpCore(
      withOptions({
        environment,
        registerLocaleFn: registerLocale(),
      }),
    ),
    provideAbpOAuth(),
    provideIdentityConfig(),
    provideSettingManagementConfig(),
    provideFeatureManagementConfig(),
    provideThemeLeptonX(),
    provideSideMenuLayout(),
    provideLogo(withEnvironmentOptions(environment)),
    provideAccountConfig(),
    provideAbpThemeShared(),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: NotificationInterceptor,
      multi: true,
    },
    {
      provide: IDENTITY_ENTITY_ACTION_CONTRIBUTORS,
      useValue: PUBLIC_PROFILE_ENTITY_ACTION_CONTRIBUTORS,
      multi: true,
    },
    {
      provide: IDENTITY_ENTITY_PROP_CONTRIBUTORS,
      useValue: PUBLIC_PROFILE_ENTITY_PROP_CONTRIBUTORS,
      multi: true,
    },
  ]
};
