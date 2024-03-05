import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AccountService } from '../services/account.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private accountService: AccountService, private router: Router) {}

  canActivate(): Observable<boolean> | Promise<boolean> | boolean {
    if (this.accountService.isUserAuthenticated()) {
      return true; // Allow navigation to the protected route
    } else {
      this.router.navigate(['/login']); // Navigate to login page
      return false; // Block navigation to the protected route
    }
  }
}

