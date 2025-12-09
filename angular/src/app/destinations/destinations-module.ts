import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router'; 
import { DESTINATIONS_ROUTES } from './destinations.routes'; 


import { DestinationsListComponent } from './destinations-list/destinations-list';

@NgModule({
  declarations: [
    DestinationsListComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    
    RouterModule.forChild(DESTINATIONS_ROUTES) 
  ]
})
export class DestinationsModule { }