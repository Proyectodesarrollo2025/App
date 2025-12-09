import { authGuard, permissionGuard } from '@abp/ng.core';
import { Routes } from '@angular/router';

export const APP_ROUTES: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./home/home.component').then(c => c.HomeComponent),
  },
  
  
  {
    path: 'destinations',
    loadChildren: () => import('./destinations/destinations-module').then(m => m.DestinationsModule),
    canActivate: [authGuard] // cambio
  },
  
  {
    path: 'account',
    loadChildren: () => import('@abp/ng.account').then(c => c.createRoutes()),
  },
];