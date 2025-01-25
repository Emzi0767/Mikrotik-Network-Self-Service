import { Component, Inject, Optional } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { CoreModule } from '../../../core.module';
import { INewDhcpLeaseForm } from '../../../types/new-dhcp-lease.form';
import { DhcpLease } from '../../../proto/dhcp.pb';
import { Nullable } from '../../../types/nullable.type';
import { FormInputComponent } from "../../partial/form-input/form-input.component";

@Component({
  selector: 'app-add-dhcp-lease',
  standalone: true,
  imports: [
    CoreModule,
    FormInputComponent
],
  templateUrl: './add-dhcp-lease.template.html',
  styles: ``
})
export class AddDhcpLeaseComponent {
  dhcpLeaseForm: FormGroup<INewDhcpLeaseForm>;

  constructor(
    private dialogRef: MatDialogRef<AddDhcpLeaseComponent>,
    private formBuilder: FormBuilder,
    @Optional() @Inject(MAT_DIALOG_DATA) lease?: DhcpLease,
  ) {
    this.dhcpLeaseForm = this.formBuilder.group<INewDhcpLeaseForm>({
      macAddress:  new FormControl<Nullable<string>>(null, { validators: [ Validators.required, Validators.pattern(/^(?:[a-fA-F0-9]{2}(?<sep>[:-]?))(?:[a-fA-F0-9]{2}\k<sep>){4}([a-fA-F0-9]{2})$/) ] }),
      ipAddress:   new FormControl<Nullable<string>>(null, { validators: [ Validators.required, Validators.pattern(/^(?:[0-9]{1,3}\.){3}(?:[0-9]{1,3})$/) ] }),
    });

    if (lease !== undefined) {
      this.dhcpLeaseForm.patchValue({
        macAddress: lease.macAddress,
      });
    }
  }

  cancel(): void {
    this.dhcpLeaseForm.disable();
    this.dialogRef.close(null);
  }

  confirm(): void {
    this.dhcpLeaseForm.disable();
    this.dhcpLeaseForm.markAllAsTouched();
    if (!this.dhcpLeaseForm.valid)
      return;

    this.dhcpLeaseForm.disable();
    this.dialogRef.close(this.dhcpLeaseForm.value);
  }
}
