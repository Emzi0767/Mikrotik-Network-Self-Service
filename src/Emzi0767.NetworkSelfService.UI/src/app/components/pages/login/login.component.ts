import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';

import { CoreModule } from '../../../core.module';
import { ILoginForm } from '../../../types/login-form.form';
import { Nullable } from '../../../types/nullable';
import { AuthenticationProviderService } from '../../../services/authentication-provider.service';
import { AuthenticationClientService } from '../../../services/grpc/authentication-client.service';
import { FormInputComponent } from "../../partial/form-input/form-input.component";
import { SnackService } from '../../../services/snack.service';
import { Error, ErrorCode } from '../../../proto/result.pb';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CoreModule,
    FormInputComponent,
  ],
  templateUrl: './login.template.html',
  styles: ``
})
export class LoginComponent {
  loginForm: FormGroup<ILoginForm>;

  constructor(
    private snackService: SnackService,
    private formBuilder: FormBuilder,
    private authenticationProvider: AuthenticationProviderService,
    private authenticationClient: AuthenticationClientService,
    private router: Router,
  ) {
    this.loginForm = this.formBuilder.group<ILoginForm>({
      username: new FormControl<Nullable<string>>("", { validators: [ Validators.required, ], }),
      password: new FormControl<Nullable<string>>("", { validators: [ Validators.required, ], }),
      remember: new FormControl<Nullable<boolean>>(false, { validators: [], }),
    });
  }

  submit(): void {
    this.loginForm.markAllAsTouched();
    if (!this.loginForm.valid) {
      this.snackService.show("Please fill in the login credentials.", "Dismiss");
      return;
    }

    this.loginForm.disable();

    const { username, password, remember } = this.loginForm.value;
    this.authenticationClient.authenticate(username!, password!, remember!)
      .subscribe({
        next: x => {
          this.authenticationProvider.updateSession(x.session!);
          this.router.navigate([ '/' ]);
        },
        error: x => {
          const err = x as Error;
          this.loginForm.enable();
          this.snackService.show("Could not log in: " + ErrorCode[err.code], "Dismiss");
        }
      });
  }
}
