import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ErrorCode } from '../proto/result.pb';

@Injectable({
  providedIn: 'root'
})
export class SnackService {

  constructor(
    private snackBar: MatSnackBar,
  ) { }

  show(msg: string, act: string): void {
    this.snackBar.open(
      msg,
      act,
      {
        horizontalPosition: 'center',
        verticalPosition: 'top',
        duration: 5000,
      }
    );
  }

  showError(msg: string, err: ErrorCode, act: string): void {
    const errMsg = {
      [ErrorCode.UNKNOWN]: "Unknown error",
      [ErrorCode.INVALID_CREDENTIALS]: "Invalid credentials specified",
      [ErrorCode.SESSION_INVALID_OR_EXPIRED]: "Session has expired, try logging in again",
    }[err];

    this.show(msg + errMsg, act);
  }
}
