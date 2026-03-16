import { Injectable } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable, Subject } from 'rxjs';

export interface AppNotificationDto {
    id: string;
    userId: string;
    title: string;
    message: string;
    isRead: boolean;
    type: string;
    creationTime: string;
}

export interface PagedResultDto<T> {
    totalCount: number;
    items: T[];
}

@Injectable({
    providedIn: 'root'
})
export class NotificationService {
    public notificationUpdated$ = new Subject<void>();

    constructor(private restService: RestService) { }

    getNotifications(isRead?: boolean): Observable<PagedResultDto<AppNotificationDto>> {
        const query = isRead !== undefined ? `?IsRead=${isRead}` : '';
        return this.restService.request<void, PagedResultDto<AppNotificationDto>>({
            method: 'GET',
            url: `/api/app/notification${query}`
        },
            { apiName: 'Default' });
    }

    getUnreadCount(): Observable<number> {
        return this.restService.request<void, number>({
            method: 'GET',
            url: `/api/app/notification/unread-count`
        },
            { apiName: 'Default' });
    }

    markAsRead(id: string): Observable<void> {
        return this.restService.request<void, void>({
            method: 'POST',
            url: `/api/app/notification/${id}/mark-as-read`
        },
            { apiName: 'Default' });
    }

    markAllAsRead(): Observable<void> {
        return this.restService.request<void, void>({
            method: 'POST',
            url: `/api/app/notification/mark-all-as-read`
        },
            { apiName: 'Default' });
    }
}
