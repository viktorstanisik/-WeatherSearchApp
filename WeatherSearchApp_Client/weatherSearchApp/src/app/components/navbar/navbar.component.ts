import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { finalize } from 'rxjs';
import { ErrorMessages } from 'src/app/helpers/errorMessages';
import { AccountService } from 'src/app/services/account.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  constructor(public accountService: AccountService, private router: Router, private toastr: ToastrService) {

   }
   showLogoutButton: boolean = true
   showAccountPage: boolean = false;


  ngOnInit(): void {
    const userHasToken = this.accountService.isUserAuthenticated(); {
      if(!userHasToken) {
        this.showLogoutButton = false
      } else {
        this.showLogoutButton = true;
      }
    }
  }

  logout() {
    this.accountService.logout()
      .pipe(
        finalize(() => {
          this.router.navigateByUrl('/');
          this.accountService.setCurrentUser(false);
          this.showLogoutButton = false;
        })
      )
      .subscribe({
        next: response => {
          if (response.success) {
            this.toastr.success(ErrorMessages.logoutSuccess);
          } else {
            this.toastr.error(response.errorMessage || ErrorMessages.genericUnknownError);
          }
        }
      });
  }

  goToAccountPage() {
    this.showAccountPage = !this.showAccountPage

    this.router.navigateByUrl('account-information')
  }

  goToHomePage() {
    this.showAccountPage = !this.showAccountPage
    this.router.navigateByUrl('getweather')
  }
}

