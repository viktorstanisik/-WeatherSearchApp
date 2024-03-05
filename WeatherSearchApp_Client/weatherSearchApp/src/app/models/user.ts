export interface User {
  email: string;
  isTwoFactorEnabled:boolean;
  isAccountLocked: boolean;
  isAccountConfirmed: boolean;
  isTokenGenerated: boolean;
}
