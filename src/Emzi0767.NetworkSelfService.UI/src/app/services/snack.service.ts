import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

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
        //duration: 5000,
      }
    );
  }
}
