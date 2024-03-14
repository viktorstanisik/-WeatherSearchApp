import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import {  finalize, tap } from 'rxjs';
import { ErrorMessages } from 'src/app/helpers/errorMessages';
import { AccountInfoModel } from 'src/app/models/accountInfoModel';
import { ChangeTwoFactorAuthStatusModel } from 'src/app/models/changeTwoFactorAuthStatusModel';
import { AccountService } from 'src/app/services/account.service';

@Component({
  selector: 'app-account-info',
  templateUrl: './account-info.component.html',
  styleUrls: ['./account-info.component.css']
})
export class AccountInfoComponent implements OnInit {

  constructor(private accountService: AccountService, private toastr: ToastrService, private router: Router) { }
  showSpinner: boolean = false;
  twoFactorAuthText: string = 'Two Factor Authentication';
  accountInfoModel: AccountInfoModel = {
    firstName: '',
    lastName: '',
    email: '',
    isTwoFactorEnabled: false
  }
  changeTwoFactorAuthStatusModel: ChangeTwoFactorAuthStatusModel = {
    email: '',
    password: ''
  }

  showConfirmationScreen: boolean = false
  is2faEnabledText: string = ''

  ngOnInit(): void {
   this.getAccountInfo(); // Call the method to get account info when component initializes
  }

    getAccountInfo() {
      this.showSpinner = true;
      this.accountService.getAccountInfo()
        .pipe(
          tap(response => {
            if (response.success) {
              this.accountInfoModel = response.data;
              this.changeTwoFactorAuthStatusModel.email = this.accountInfoModel.email;
              this.setTwoFactorButtonText(this.accountInfoModel.isTwoFactorEnabled);
            } else {
              const errorMessage = response.errorMessage || ErrorMessages.genericUnknownError;
              this.toastr.error(errorMessage, 'Error');
            }
          }),
          finalize(() => {
            this.showSpinner = false;
          })
        )
        .subscribe();
    }

    changeTwoFactorAuthenticationStatus() {
      this.showSpinner = true;
      this.accountService.changeTwoFactorAuthenticationStatus(this.changeTwoFactorAuthStatusModel)
        .pipe(
          tap(response => {
            if (response.success && response.data) {
              this.toastr.success(ErrorMessages.twoFactorAuthenticationStatus(this.is2faEnabledText), 'Success');
              this.setShowConfirmationScreen();
              this.getAccountInfo();

            } else {
              const errorMessage = response.errorMessage || ErrorMessages.genericUnknownError;
              this.toastr.error(errorMessage, 'Error');
              this.setShowConfirmationScreen();
              this.getAccountInfo();


            }
          }),
          finalize(() => {
            this.showSpinner = false;
          })
        )
        .subscribe();
    }


    setTwoFactorButtonText(hasTwoFactorEnabled: boolean) {
      this.is2faEnabledText = hasTwoFactorEnabled ? 'Disable' : 'Enable'
      this.twoFactorAuthText = this.is2faEnabledText + ' ' + this.twoFactorAuthText

    }

    setShowConfirmationScreen() {
      this.showConfirmationScreen = !this.showConfirmationScreen
    }

}
