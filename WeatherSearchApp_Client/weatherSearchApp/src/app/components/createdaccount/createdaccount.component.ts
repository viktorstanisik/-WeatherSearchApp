import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription, catchError, of, retry, switchMap, tap } from 'rxjs';
import { VerifyAccount } from 'src/app/models/verifyAccount';
import { AccountService } from 'src/app/services/account.service';
import { environment } from 'src/environments/environment';
import { ToastrService } from 'ngx-toastr';
import { ErrorMessages } from 'src/app/helpers/errorMessages';

@Component({
  selector: 'app-createdaccount',
  templateUrl: './createdaccount.component.html',
  styleUrls: ['./createdaccount.component.css'],
})
export class CreatedaccountComponent implements OnInit {
  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private accountService: AccountService,
    private toastr: ToastrService
  ) {}

  verifyAccountModel: VerifyAccount = { hashedToken: '' };
  accountIsNotVerified: boolean = false;
  showValidationMessage: boolean = false;
  private subscription: Subscription | undefined;
  showSpinner: boolean = false;

  ngOnInit(): void {
    this.showSpinner = true;
    let errorMessageBasedOnResponse: string = '';
    this.subscription = this.route.queryParamMap
      .pipe(
        switchMap((params) => {
          const urlParam = params.get(environment.urlParam);

          this.verifyAccountModel.hashedToken = urlParam || '';

          return this.confirmAccount(this.verifyAccountModel).pipe(
            retry(environment.failedHttpRetryTimes) // Retry the request up to 3 times
          );
        }),
        tap((response) => {
          if (response.success) {
            this.accountSuccessfullVerification();
          } else {
            this.showSpinner = false;

            errorMessageBasedOnResponse = ErrorMessages.accountNotVerified;

            this.toastr.show(errorMessageBasedOnResponse, 'Error');

            this.accountIsNotVerified = true;
          }

          this.showValidationMessage = true;
        }),
        catchError(() => {
          this.showSpinner = false;
          this.showValidationMessage = true;

          this.router.navigateByUrl('/');

          return of(null);
        })
      )
      .subscribe();
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  confirmAccount(hashedToken: VerifyAccount) {
    return this.accountService.confirmAccount(hashedToken);
  }

  accountSuccessfullVerification() {
    setTimeout(() => {
      this.showSpinner = false;
      this.showValidationMessage = true;
      this.toastr.success(ErrorMessages.accountVerified);
      this.router.navigateByUrl('/');
    }, 3000);
  }
}
