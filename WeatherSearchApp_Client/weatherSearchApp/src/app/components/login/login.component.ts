import { Component, OnInit } from '@angular/core';
import { ErrorMessages } from 'src/app/helpers/errorMessages';
import { LoginModel } from 'src/app/models/loginModel';
import { AccountService } from 'src/app/services/account.service';
import { ToastrService } from 'ngx-toastr';
import { VerifyTwoFactorAuthModel } from 'src/app/models/verifyTwoFactorAuthModel';
import { User } from 'src/app/models/user';
import { environment } from 'src/environments/environment';
import { ServiceResponse } from 'src/app/models/serviceResponse';
import { finalize, tap } from 'rxjs';
import { ResendSecuirtyCode } from 'src/app/models/resendSecuirtyCode';
import { LoginResponseModel } from 'src/app/models/loginResponseModel';
import { Router } from '@angular/router';
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit {
  constructor(
    public accountService: AccountService,
    private toastr: ToastrService,
    private router: Router
  ) {}

  showSecurityCodeField: boolean = false;
  showSpinner: boolean = false;

  loginModel: LoginModel = {
    email: '',
    password: '',
  };

  verifyTwoFactorModel: VerifyTwoFactorAuthModel = {
    email: '',
    securityCode: '',
  };

  ngOnInit(): void {    }

  login() {
    this.showSpinner = true;
    this.accountService
      .login(this.loginModel)
      .subscribe((response: ServiceResponse<LoginResponseModel>) => {
        if (response.success && response.data) {
          const user: User = response.data;
          if (user.isTwoFactorEnabled && !user.isTokenGenerated) {
            this.showSecurityCodeField = this.userHasTwoFactorEnabled(user);
          } else {
            this.handleSuccessfulLogin(user);
          }
        } else {
          const errorMessage =
            response.errorMessage || ErrorMessages.genericUnknownError;
          this.toastr.error(errorMessage, 'Error');
        }
      })
      .add(() => {
        this.showSpinner = false;
      });
  }

  private userHasTwoFactorEnabled(user: User): boolean {
    return user && user.isTwoFactorEnabled && !user.isTokenGenerated;
  }

  verifyTheSecurityCode() {
    this.showSpinner = true;
    this.verifyTwoFactorModel.email = this.loginModel.email;
    this.accountService
      .verifyTwoFactor(this.verifyTwoFactorModel)
      .subscribe((response: ServiceResponse<LoginResponseModel>) => {
        if (response.success) {
          const user: User = response.data;
          this.handleSuccessfulLogin(user);
          this.router.navigateByUrl('getweather');
        } else {
          const errorMessage =
            response.errorMessage || ErrorMessages.genericUnknownError;
          this.toastr.error(errorMessage, 'Error');
          if (response.data.isAccountLocked) {
            this.router.navigateByUrl('/');
            this.showSecurityCodeField = false;
          }
        }
      })
      .add(() => {
        this.showSpinner = false;
      });
  }

  generateRegex() {
    let lengthOfSecurityCode = environment.securityCodeLength;
    let customRegex = '^[a-zA-Z0-9]{' + lengthOfSecurityCode + '}$';
    return new RegExp(customRegex);
  }

  handleSuccessfulLogin(userFromResponse: User) {
    if (userFromResponse && userFromResponse.isTokenGenerated) {
      const user: User = userFromResponse;
      this.accountService.setCurrentUser(user !== null);
      this.router.navigateByUrl('getweather');

      this.toastr.success(ErrorMessages.logInSuccess, 'Success');
    }
  }

  resendCode() {
    this.showSpinner = true;

    const model: ResendSecuirtyCode = {
      email: this.loginModel.email,
    };

    this.accountService
      .resendSecurityCode(model)
      .pipe(
        tap((response: ServiceResponse<boolean>) => {
          if (response.success && response.data) {
            this.toastr.success(ErrorMessages.secuirtyCodeResendSuccess);
          } else {
            this.toastr.error(ErrorMessages.securityCodeFailedSend);
          }
        }),
        finalize(() => {
          this.showSpinner = false;
        })
      )
      .subscribe();
  }
}
