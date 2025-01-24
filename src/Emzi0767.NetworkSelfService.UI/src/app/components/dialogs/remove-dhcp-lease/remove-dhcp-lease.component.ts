import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { CoreModule } from '../../../core.module';
import { DhcpLease } from '../../../proto/dhcp.pb';
import { IMetaTableEntry } from '../../../types/meta-table-entry.interface';

@Component({
  selector: 'app-remove-dhcp-lease',
  standalone: true,
  imports: [
    CoreModule,
  ],
  templateUrl: './remove-dhcp-lease.template.html',
  styles: ``
})
export class RemoveDhcpLeaseComponent {
  metaTableEntries!: IMetaTableEntry[];

  constructor(
    private dialogRef: MatDialogRef<RemoveDhcpLeaseComponent>,
    @Inject(MAT_DIALOG_DATA) lease: DhcpLease,
  ) {
    this.metaTableEntries = [
      {
        property: "MAC Address",
        value: lease.macAddress,
      },
      {
        property: "IP Address",
        value: lease.ipAddress,
      },
    ];
  }

  cancel(): void {
    this.dialogRef.close(false);
  }

  confirm(): void {
    this.dialogRef.close(true);
  }
}
