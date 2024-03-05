
export interface LoginResponseModel {
  email: string;
  isTwoFactorEnabled:boolean;
  isAccountLocked: boolean;
  isAccountConfirmed: boolean;
  isTokenGenerated: boolean;
}
