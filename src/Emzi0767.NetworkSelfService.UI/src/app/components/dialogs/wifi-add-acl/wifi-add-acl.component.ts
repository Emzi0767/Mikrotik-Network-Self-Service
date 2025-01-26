import { Component, Inject, Optional } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { CoreModule } from '../../../core.module';
import { FormInputComponent } from '../../partial/form-input/form-input.component';
import { WifiAcl, WifiWeekday } from '../../../proto/wifi.pb';
import { Nullable, NullableOptional, NullableProperties } from '../../../types/nullable.type';
import { INewWifiAclForm, WifiWeekdaySelector } from '../../../types/new-wifi-acl.form';
import { FormFor } from '../../../types/form.type';
import { requireIf } from '../../../validators/require-if.validator';

@Component({
  selector: 'app-wifi-add-acl',
  standalone: true,
  imports: [
    CoreModule,
    FormInputComponent,
  ],
  templateUrl: './wifi-add-acl.template.html',
  styleUrls: [
    './wifi-add-acl.style.scss',
  ],
})
export class WifiAddAclComponent {
  wifiAclForm: FormGroup<INewWifiAclForm>;
  acl?: WifiAcl | null;
  create: boolean;

  constructor(
    private dialogRef: MatDialogRef<WifiAddAclComponent>,
    private formBuilder: FormBuilder,
    @Inject(MAT_DIALOG_DATA) aclData: { acl?: WifiAcl | null, create: boolean, },
  ) {
    this.wifiAclForm = this.formBuilder.group<INewWifiAclForm>({
      macAddress:                 new FormControl<Nullable<string>>                   (null,      { validators: [ Validators.required, Validators.pattern(/^(?:[a-fA-F0-9]{2}(?<sep>[:-]?))(?:[a-fA-F0-9]{2}\k<sep>){4}([a-fA-F0-9]{2})$/), ], }),
      comment:                    new FormControl<Nullable<string>>                   (null,      { validators: [ Validators.required, ], }),
      hasPrivatePassword:         new FormControl<Nullable<boolean>>                  (false,     { validators: [], }),
      privatePassword:            new FormControl<NullableOptional<string>>           (undefined, { validators: [ requireIf<INewWifiAclForm>('hasPrivatePassword'), Validators.minLength(8), Validators.maxLength(63), ], }),
      hasSchedule:                new FormControl<Nullable<boolean>>                  (false,     { validators: [], }),
      scheduleStart:              new FormControl<NullableOptional<Date>>             (undefined, { validators: [ requireIf<INewWifiAclForm>('hasSchedule') ], }),
      scheduleEnd:                new FormControl<NullableOptional<Date>>             (undefined, { validators: [ requireIf<INewWifiAclForm>('hasSchedule') ] }),
      weekdays:                   this.formBuilder.group<FormFor<NullableProperties<WifiWeekdaySelector>>>  ({
        MONDAY:                   new FormControl<Nullable<boolean>>                  (false,     { validators: [], }),
        TUESDAY:                  new FormControl<Nullable<boolean>>                  (false,     { validators: [], }),
        WEDNESDAY:                new FormControl<Nullable<boolean>>                  (false,     { validators: [], }),
        THURSDAY:                 new FormControl<Nullable<boolean>>                  (false,     { validators: [], }),
        FRIDAY:                   new FormControl<Nullable<boolean>>                  (false,     { validators: [], }),
        SATURDAY:                 new FormControl<Nullable<boolean>>                  (false,     { validators: [], }),
        SUNDAY:                   new FormControl<Nullable<boolean>>                  (false,     { validators: [], }),
        UNKNOWN:                  new FormControl<Nullable<boolean>>                  (false,     { validators: [], }),
      }),
      enable:                     new FormControl<Nullable<boolean>>                  (true,      { validators: [], }),
    });

    this.acl = aclData.acl;
    this.create = aclData.create;
    if (this.acl !== undefined && this.acl !== null) {
      this.wifiAclForm.patchValue({
        macAddress: this.acl.macAddress,
        comment: this.acl.comment,
        hasPrivatePassword: this.acl.privatePassword !== undefined && this.acl.privatePassword !== null && this.acl.privatePassword.length > 0,
        hasSchedule: this.acl.timeRestriction !== undefined && this.acl.timeRestriction !== null,
        scheduleStart: new Date(this.acl.timeRestriction?.start?.seconds!),
        scheduleEnd: new Date(this.acl.timeRestriction?.end?.seconds!),
        weekdays: {
          MONDAY: this.acl.timeRestriction?.days.includes(WifiWeekday.MONDAY),
          TUESDAY: this.acl.timeRestriction?.days.includes(WifiWeekday.TUESDAY),
          WEDNESDAY: this.acl.timeRestriction?.days.includes(WifiWeekday.WEDNESDAY),
          THURSDAY: this.acl.timeRestriction?.days.includes(WifiWeekday.THURSDAY),
          FRIDAY: this.acl.timeRestriction?.days.includes(WifiWeekday.FRIDAY),
          SATURDAY: this.acl.timeRestriction?.days.includes(WifiWeekday.SATURDAY),
          SUNDAY: this.acl.timeRestriction?.days.includes(WifiWeekday.SUNDAY),
        },
        enable: this.acl.isEnabled,
      });
    }
  }

  cancel(): void {
    this.wifiAclForm.disable();
    this.dialogRef.close(null);
  }

  confirm(): void {
    this.wifiAclForm.markAllAsTouched();
    this.wifiAclForm.updateValueAndValidity();
    if (!this.wifiAclForm.valid)
      return;

    this.wifiAclForm.disable();
    this.dialogRef.close(this.wifiAclForm.value);
  }
}
