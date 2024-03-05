export const ErrorMessages = {
  accountNotVerified: 'Account Not verified. Try Again',
  tooManyRequest: 'Too many requests. Please try again later',
  accountVerified: 'Account Succesfully verified',
  genericErrorWhileVerifyingAccount: 'An error occurred while verifying your account. Please try again.',
  registerSuccessEmailSend: 'Registration successful. Please verify your account',
  passwordsDontMatch: 'Passwords do not match',
  invalidEmailOrPassword: 'Incorrect email or password!!',
  genericUnknownError: 'An unknown error occurred',
  resourceNotFound: 'Resource not found',
  unauthorizedAccess:'Unauthorized access',
  logInSuccess: 'Logged in successfully',
  secuirtyCodeResendSuccess: 'Secuirty code resent successfully',
  securityCodeFailedSend: 'Failed to resend TFA code. Please try again later',
  serverError: 'Server error occurred. Please try again later',
  logoutSuccess: 'Logout successful',
  twoFactorAuthenticationStatus: (is2faEnabledText: string) => `You have successfully ${is2faEnabledText} two-factor authentication`,

};
