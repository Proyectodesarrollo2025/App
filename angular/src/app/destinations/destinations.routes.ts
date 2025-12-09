import { Routes } from '@angular/router';
import { authGuard } from '@abp/ng.core'; 
import { DestinationsListComponent } from './destinations-list/destinations-list';



export const DESTINATIONS_ROUTES: Routes = [
  {
    path: '', 
    canActivate: [authGuard], 
    component: DestinationsListComponent, 
  },
];