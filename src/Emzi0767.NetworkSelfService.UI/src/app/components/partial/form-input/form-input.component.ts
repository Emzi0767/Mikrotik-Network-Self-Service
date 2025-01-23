import { Component, Input } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { CoreModule } from '../../../core.module';
import { Nullable } from '../../../types/nullable';
import { InputType } from '../../../types/input-type.enum';

@Component({
  selector: 'app-form-input',
  standalone: true,
  imports: [
    CoreModule,
  ],
  templateUrl: './form-input.template.html',
})
export class FormInputComponent {
  @Input({ required: true }) controlName!: string;
  @Input({ required: true }) form!: FormGroup<any>;
  @Input({ required: true }) title!: string;
  @Input() type: keyof typeof InputType = 'TEXT';

  get control(): FormControl<Nullable<any>> {
    return this.form.controls[this.controlName]! as FormControl<Nullable<any>>;
  }
}
