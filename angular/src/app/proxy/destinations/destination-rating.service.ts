import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { DestinationRatingDto } from '../application/contracts/destinations/models';

@Injectable({
  providedIn: 'root',
})
export class DestinationRatingService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  rateDestination = (destinationId: string, score: number, comment: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/destination-rating/rate-destination/${destinationId}`,
      params: { score, comment },
    },
    { apiName: this.apiName,...config });

  updateRating = (id: string, score: number, comment: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'PUT',
      url: `/api/app/destination-rating/${id}/rating`,
      params: { score, comment },
    },
    { apiName: this.apiName,...config });

  deleteRating = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/destination-rating/${id}/rating`,
    },
    { apiName: this.apiName,...config });

  getRatings = (destinationId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinationRatingDto[]>({
      method: 'GET',
      url: `/api/app/destination-rating/ratings/${destinationId}`,
    },
    { apiName: this.apiName,...config });

  getAverageRating = (destinationId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, number>({
      method: 'GET',
      url: `/api/app/destination-rating/average-rating/${destinationId}`,
    },
    { apiName: this.apiName,...config });

  getMyRatings = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinationRatingDto[]>({
      method: 'GET',
      url: '/api/app/destination-rating/my-ratings',
    },
    { apiName: this.apiName,...config });
}