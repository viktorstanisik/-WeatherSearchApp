import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { RegisterModel } from '../models/registerModel';
import { LoginModel } from '../models/loginModel';
import { ServiceResponse } from '../models/serviceResponse';
import { VerifyAccount } from '../models/verifyAccount';
import { ResendSecuirtyCode } from '../models/resendSecuirtyCode';
import { User } from '../models/user';
import { VerifyTwoFactorAuthModel } from '../models/verifyTwoFactorAuthModel';
import { LoginResponseModel } from '../models/loginResponseModel';
import { AccountInfoModel } from '../models/accountInfoModel';
import { ChangeTwoFactorAuthStatusModel } from '../models/changeTwoFactorAuthStatusModel';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private hasLoggedUserSubject: BehaviorSubject<boolean>;
  private requestIsFromConfirmingAccountSubject: BehaviorSubject<boolean>;



  constructor(private http: HttpClient) {
    this.hasLoggedUserSubject = new BehaviorSubject<boolean>(false);
    this.requestIsFromConfirmingAccountSubject = new BehaviorSubject<boolean>(false);

  }
  get hasLoggedUser$() {
    return this.hasLoggedUserSubject.asObservable();
  }
get isRequestIsFromConfirmingAccountSubject$() {
  return this.requestIsFromConfirmingAccountSubject.asObservable();
}

  setCurrentUser(userIsLogged: boolean) {
    if(this.isUserAuthenticated() && userIsLogged === null) {
      this.hasLoggedUserSubject.next(true)
    }
    this.hasLoggedUserSubject.next(userIsLogged);
  }

  isRequestIsFromConfirmingAccountSubject(isFromRegister: boolean) {
    this.requestIsFromConfirmingAccountSubject.next(isFromRegister);

  }

  login(loginModel: LoginModel): Observable<ServiceResponse<LoginResponseModel>> {
    return this.http.post<ServiceResponse<LoginResponseModel>>(environment.apiUrl + 'User/login-user', loginModel);
  }

  confirmAccount(hashedToken: VerifyAccount): Observable<ServiceResponse<boolean>> {
    return this.http.post<ServiceResponse<boolean>>(environment.apiUrl + 'User/confirm-account', hashedToken)
  }

  register(registerModel: RegisterModel): Observable<ServiceResponse<number>> {
    return this.http.post<ServiceResponse<number>>(environment.apiUrl + 'User/register-account', registerModel);
  }

  resendSecurityCode(model: ResendSecuirtyCode): Observable<ServiceResponse<boolean>> {
    return this.http.post<ServiceResponse<boolean>>(environment.apiUrl + 'User/resend-email', model);
  }

  verifyTwoFactor(verifyAuthModel: VerifyTwoFactorAuthModel): Observable<ServiceResponse<LoginResponseModel>>{
    let response = this.http.post<ServiceResponse<LoginResponseModel>>(environment.apiUrl + 'User/verify-two-factor-auth', verifyAuthModel);
    return response;
  }

  getAccountInfo(): Observable<ServiceResponse<AccountInfoModel>> {
    return this.http.post<ServiceResponse<AccountInfoModel>>(environment.apiUrl + 'User/get-logged-user-info', {});
  }

   changeTwoFactorAuthenticationStatus(changeTwoFactorAuthStatusModel: ChangeTwoFactorAuthStatusModel): Observable<ServiceResponse<boolean>> {
    return this.http.post<ServiceResponse<boolean>>(`${environment.apiUrl}User/change-two-factor-auth-status`, changeTwoFactorAuthStatusModel);
  }


  logout(): Observable<ServiceResponse<boolean>> {
   return this.http.post<ServiceResponse<boolean>>(environment.apiUrl + 'User/logout', {});
 }

 isUserAuthenticated(): Observable<boolean> {
   return this.http.post<boolean>(environment.apiUrl + 'User/isAuthenticated', {});
 }

}
