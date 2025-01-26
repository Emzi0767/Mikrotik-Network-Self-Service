import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

import { CoreModule } from '../../../core.module';

@Component({
  selector: 'app-wifi-remove-acl',
  standalone: true,
  imports: [
    CoreModule,
  ],
  templateUrl: './wifi-remove-acl.template.html',
  styles: ``
})
export class WifiRemoveAclComponent {

  constructor(
    private dialogRef: MatDialogRef<WifiRemoveAclComponent>,
  ) { }

  cancel(): void {
    this.dialogRef.close(false);
  }

  confirm(): void {
    this.dialogRef.close(true);
  }
}
