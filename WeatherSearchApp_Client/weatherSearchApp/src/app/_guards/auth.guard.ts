import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { Observable, catchError, map, of } from 'rxjs';
import { AccountService } from '../services/account.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private accountService: AccountService, private router: Router) {}


  canActivate(): Observable<boolean> {
    return this.accountService.isUserAuthenticated().pipe(
      map((isAuthenticated: boolean) => {
        if (isAuthenticated) {
          return true; // Allow navigation to the protected route
        } else {
          this.router.navigateByUrl(''); // Navigate to login page
          return false; // Block navigation to the protected route
        }
      })
    );
  }
}
