import { authGuard } from '@abp/ng.core';
import { Routes } from '@angular/router';
import { MyProfileComponent } from './my-profile/my-profile.component';
import { PublicProfileComponent } from './public-profile/public-profile.component';

export const APP_ROUTES: Routes = [
  {
    path: 'my-profile',
    component: MyProfileComponent,
    canActivate: [authGuard],
  },
  {
    path: 'public-profile/:id',
    component: PublicProfileComponent,
  },
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./home/home.component').then(c => c.HomeComponent),
  },
  {
    path: 'destinations',
    loadChildren: () => import('./destinations/destinations-module').then(m => m.DestinationsModule),
    canActivate: [authGuard],
  },
  {
    path: 'identity',
    loadChildren: () => import('@abp/ng.identity').then(m => m.createRoutes()),
  },
  {
    path: 'setting-management',
    loadChildren: () => import('@abp/ng.setting-management').then(m => m.createRoutes()),
  },
  {
    path: 'account',
    loadChildren: () => import('@abp/ng.account').then(c => c.createRoutes()),
  },
  {
    path: '**',
    redirectTo: '',
  },
];