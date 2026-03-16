import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { PublicUserProfileDto } from './models';

@Injectable({
    providedIn: 'root',
})
export class UserProfileService {
    private restService = inject(RestService);
    apiName = 'Default';

    getPublicProfile = (id: string, config?: Partial<Rest.Config>) =>
        this.restService.request<any, PublicUserProfileDto>({
            method: 'GET',
            url: `/api/app/user-profile/${id}/public-profile`,
        },
            { apiName: this.apiName, ...config });

    getMyProfile = (config?: Partial<Rest.Config>) =>
        this.restService.request<any, PublicUserProfileDto>({
            method: 'GET',
            url: '/api/app/user-profile/my-profile',
        },
            { apiName: this.apiName, ...config });

    deleteMyAccount = (config?: Partial<Rest.Config>) =>
        this.restService.request<any, void>({
            method: 'DELETE',
            url: '/api/app/user-profile/my-account',
        },
            { apiName: this.apiName, ...config });

    updateProfilePicture = (input: { fotoUrl: string }, config?: Partial<Rest.Config>) =>
        this.restService.request<any, void>({
            method: 'PUT',
            url: '/api/app/user-profile/profile-picture',
            body: input,
        },
            { apiName: this.apiName, ...config });
}
