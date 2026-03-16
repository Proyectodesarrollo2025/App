import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import { DestinationDto } from '../application/contracts/destinations/models';

@Injectable({
  providedIn: 'root',
})
export class FavoriteDestinationService {
  apiName = 'Default';

  constructor(private restService: RestService) {}

  toggleFavorite = (destinationId: string, config?: Rest.Config) =>
    this.restService.request<any, void>(
      {
        method: 'POST',
        url: `/api/app/favorite-destination/toggle-favorite`,
        body: { destinationId },
      },
      { apiName: this.apiName, ...config }
    );

  getMyFavorites = (config?: Rest.Config) =>
    this.restService.request<any, DestinationDto[]>(
      {
        method: 'GET',
        url: `/api/app/favorite-destination/my-favorites`,
      },
      { apiName: this.apiName, ...config }
    );
}
