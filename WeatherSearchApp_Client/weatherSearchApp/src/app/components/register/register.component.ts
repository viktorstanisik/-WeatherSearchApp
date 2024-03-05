import {
  Component,
  EventEmitter,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { RegisterModel } from 'src/app/models/registerModel';
import { Router } from '@angular/router';
import { AccountService } from 'src/app/services/account.service';
import { ToastrService } from 'ngx-toastr';
import { NgForm } from '@angular/forms';
import { catchError, of, retry } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ErrorMessages } from 'src/app/helpers/errorMessages';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  constructor(
    private accountService: AccountService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  @Output() cancelRegister = new EventEmitter();
  @ViewChild('registerForm') registerForm: NgForm | undefined;

  registerModel: RegisterModel = {
    firstName: '',
    lastName: '',
    password: '',
    email: '',
    confirmPassword: '',
    IsTwoFactorEnabled: false,
  };

  passwordsDontMatch: string = '';

  requestIsLoading: boolean = false;

  ngOnInit(): void {
    this.registerModel.firstName = '';
    this.registerModel.lastName = '';
    this.registerModel.password = '';
    this.registerModel.confirmPassword = '';
    this.registerModel.IsTwoFactorEnabled = false;
  }

  register() {
    this.requestIsLoading = true;
    this.passwordsDontMatch = this.passwordCheck(this.registerModel);

    if (this.passwordsDontMatch === '') {
      this.accountService
        .register(this.registerModel)
        .pipe(
          catchError(() => {
            this.requestIsLoading = false;

            return of(null); // Returning observable of null to continue the observable chain
          })
        )
        .subscribe((response) => {
          if (response && response.success) {
            // Registration success: Show success message and navigate
            this.toastr.success(
              ErrorMessages.registerSuccessEmailSend,
              'Success'
            );
            this.cancel();
          } else {
            // Registration failed: Handle specific error cases
            this.toastr.error(
              response && response.errorMessage
                ? response.errorMessage
                : ErrorMessages.genericErrorWhileVerifyingAccount,
              'Error'
            );
            this.clearForm();
          }
          this.requestIsLoading = false;
          this.accountService.isRequestIsFromConfirmingAccountSubject(true)
        });
    }
  }

  cancel() {
    this.router.navigateByUrl('');
    this.cancelRegister.emit(false);
  }

  passwordCheck(registerModel: RegisterModel): string {
    if (registerModel.password !== registerModel.confirmPassword) {
      this.requestIsLoading = false;
      this.toastr.error(ErrorMessages.passwordsDontMatch, 'Error');
      return ErrorMessages.passwordsDontMatch;
    }
    return '';
  }

  clearForm() {
    // Reset the registerModel object to clear the form fields
    this.registerModel.firstName = '';
    this.registerModel.lastName = '';
    this.registerModel.email = '';
    this.registerModel.password = '';
    this.registerModel.confirmPassword = '';
    this.registerModel.IsTwoFactorEnabled = false;
    this.passwordsDontMatch = '';
    !this.registerForm?.form.markAsPristine();
  }
}
