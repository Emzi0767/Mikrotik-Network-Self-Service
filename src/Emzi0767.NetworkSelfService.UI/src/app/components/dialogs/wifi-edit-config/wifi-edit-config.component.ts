import { Component, Inject } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { CoreModule } from '../../../core.module';
import { IWifiConfigForm } from '../../../types/wifi-config.form';
import { WifiConfigResponse } from '../../../proto/wifi.pb';
import { NullableOptional } from '../../../types/nullable.type';
import { FormInputComponent } from '../../partial/form-input/form-input.component';

@Component({
  selector: 'app-wifi-edit-config',
  imports: [
    CoreModule,
    FormInputComponent,
  ],
  templateUrl: './wifi-edit-config.template.html',
  styles: ``
})
export class WifiEditConfigComponent {
  wifiConfigForm: FormGroup<IWifiConfigForm>;

  constructor(
    private dialogRef: MatDialogRef<WifiEditConfigComponent>,
    private formBuilder: FormBuilder,
    @Inject(MAT_DIALOG_DATA) config: WifiConfigResponse,
  ) {
    this.wifiConfigForm = this.formBuilder.group<IWifiConfigForm>({
      ssid:            new FormControl<NullableOptional<string>>   (undefined, { validators: [], }),
      password:        new FormControl<NullableOptional<string>>   (undefined, { validators: [Validators.minLength(8), Validators.maxLength(63)], }),
      isolateClients:  new FormControl<NullableOptional<boolean>>  (undefined, { validators: [], }),
    });

    this.wifiConfigForm.patchValue({
      ssid: config.ssid,
      isolateClients: config.isolateClients,
    });
  }

  cancel(): void {
    this.wifiConfigForm.disable();
    this.dialogRef.close(null);
  }

  confirm(): void {
    this.wifiConfigForm.markAllAsTouched();
    if (!this.wifiConfigForm.valid)
      return;

    this.wifiConfigForm.disable();
    this.dialogRef.close(this.wifiConfigForm.value);
  }
}
