import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpResponse,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError, of } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr'; // Assuming you are using Toastr for notifications
import { Router } from '@angular/router';
import { ErrorMessages } from '../helpers/errorMessages';
import { environment } from 'src/environments/environment';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private toastr: ToastrService, private router: Router) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = ErrorMessages.genericUnknownError;

        if (error.status === 0) {
          // Handle error when status is 0 (e.g., network error)
          errorMessage = ErrorMessages.serverError;
        } else {
          if (error.error instanceof ErrorEvent) {
            // Client-side error
            errorMessage = `Error: ${error.error.message}`;
          } else {
            // Server-side error
            switch (error.status) {
              case 401:
                errorMessage = ErrorMessages.unauthorizedAccess;
                this.router.navigateByUrl('/');
                break;
              case 404:
                errorMessage = ErrorMessages.resourceNotFound;
                this.router.navigateByUrl('/');
                break;
              case 429:
                errorMessage = ErrorMessages.tooManyRequest;
                this.router.navigateByUrl('/');
                break;
              default:
                errorMessage = `Error ${error.status}: ${error.message}`;
                break;
            }
          }
        }

        // Display error message using Toastr
        this.toastr.error(errorMessage, 'Error');
        return throwError(errorMessage);
      }),
      retry(environment.failedHttpRetryTimes) // Retry the request up to 3 times in case of failure
    );
  }
}
