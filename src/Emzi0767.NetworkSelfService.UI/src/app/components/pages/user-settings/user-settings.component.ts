import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';

import { CoreModule } from '../../../core.module';
import { IPasswordUpdateForm } from '../../../types/password-update-form.interface';
import { Nullable } from '../../../types/nullable.type';
import { SnackService } from '../../../services/snack.service';
import { AuthenticationClientService } from '../../../services/grpc/authentication-client.service';
import { AuthenticationProviderService } from '../../../services/authentication-provider.service';
import { PasswordUpdateRequest } from '../../../proto/auth.pb';
import { ErrorCode } from '../../../proto/result.pb';
import { FormInputComponent } from '../../partial/form-input/form-input.component';

@Component({
  selector: 'app-user-settings',
  standalone: true,
  imports: [
    CoreModule,
    FormInputComponent,
  ],
  templateUrl: './user-settings.template.html',
  styles: ``
})
export class UserSettingsComponent {
  passwordUpdateForm: FormGroup<IPasswordUpdateForm>;

  constructor(
    private formBuilder: FormBuilder,
    private snackService: SnackService,
    private authenticationClient: AuthenticationClientService,
    private authentication: AuthenticationProviderService,
  ) {
    this.passwordUpdateForm = this.formBuilder.group<IPasswordUpdateForm>({
          current: new FormControl<Nullable<string>>("", { validators: [ Validators.required, ], }),
          update:  new FormControl<Nullable<string>>("", { validators: [ Validators.required, ], }),
          confirm: new FormControl<Nullable<string>>("", { validators: [ Validators.required, ], }),
    });
  }

  submit(): void {
    this.passwordUpdateForm.markAllAsTouched();
    if (!this.passwordUpdateForm.valid) {
      this.snackService.show("Please fill in the login credentials.", "Dismiss");
      return;
    }

    this.passwordUpdateForm.disable();

    const { current, update, confirm } = this.passwordUpdateForm.value;
    this.authenticationClient.updatePassword(
      this.authentication.authenticationToken!,
      new PasswordUpdateRequest({
        currentPassword: current!,
        newPassword: update!,
        confirmPassword: confirm!,
      }),
    )
      .subscribe({
        next: x => {
          this.passwordUpdateForm.enable();
          if (!x.isSuccess) {
            this.snackService.showError("Could not update password: ", x.error!.code, "Dismiss");
          } else {
            this.snackService.show("Password updated successfully!", "Dismiss");
            this.passwordUpdateForm.reset();
          }
        },
        error: err => {
          this.passwordUpdateForm.enable();
          this.snackService.showError("Could not update password: ", err.code, "Dismiss");
        }
      });
  }
}
