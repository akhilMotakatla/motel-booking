import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { NotificationService } from '../services/notification.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const notify = inject(NotificationService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 0) {
        notify.error('Unable to connect to the server. Please check your connection.');
      } else if (error.status === 403) {
        notify.error('You do not have permission to perform this action.');
      } else if (error.status === 500) {
        notify.error('An unexpected server error occurred. Please try again.');
      }
      return throwError(() => error);
    })
  );
};
