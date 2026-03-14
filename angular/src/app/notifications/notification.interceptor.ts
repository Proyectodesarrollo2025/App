import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { NotificationService } from './notification.service';

@Injectable()
export class NotificationInterceptor implements HttpInterceptor {

  constructor(private notificationService: NotificationService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      tap((event: HttpEvent<any>) => {
        if (event instanceof HttpResponse) {
          // Check if the successful request was related to updating the profile or saving/updating a destination
          const isProfileUpdate = (request.url.includes('/api/account/my-profile') && request.method === 'PUT') ||
                                  (request.url.includes('/api/account/my-profile/change-password') && request.method === 'POST');
          const isDestinationCreate = request.url.includes('/api/app/destination') && request.method === 'POST';
          const isDestinationUpdate = request.url.includes('/api/app/destination') && request.method === 'PUT';
          const isFavoriteToggle = request.url.includes('/api/app/favorite-destination/toggle-favorite') && request.method === 'POST';

          if (isProfileUpdate || isDestinationCreate || isDestinationUpdate || isFavoriteToggle) {
            // Give a tiny delay for backend transaction to fully commit and emit the refresh event
            setTimeout(() => {
                this.notificationService.notificationUpdated$.next();
            }, 500);
          }
        }
      })
    );
  }
}
