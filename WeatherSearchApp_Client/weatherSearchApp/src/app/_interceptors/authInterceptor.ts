// http.interceptor.ts
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler) {
    // Clone the request and add the HTTP-only cookie
    const clonedReq = req.clone({
      withCredentials: true
    });

    return next.handle(clonedReq);
  }
}
