import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { IProblemData } from '../../../types/problem-data.type';
import { CoreModule } from '../../../core.module';

@Component({
  selector: 'app-dhcp-confirm',
  imports: [
    CoreModule,
  ],
  templateUrl: './dhcp-confirm.template.html',
  styles: ``
})
export class DhcpConfirmComponent {
  constructor(
    private dialogRef: MatDialogRef<DhcpConfirmComponent>,
    @Inject(MAT_DIALOG_DATA) public problemData: IProblemData,
  ) { }

  cancel(): void {
    this.dialogRef.close(false);
  }

  confirm(): void {
    this.dialogRef.close(true);
  }
}
