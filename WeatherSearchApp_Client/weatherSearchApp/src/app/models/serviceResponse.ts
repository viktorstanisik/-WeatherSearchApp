export interface ServiceResponse<T> {
  message(message: any, arg1: string): unknown;
  data: T;
  success: boolean;
  errorMessage: string;
}
