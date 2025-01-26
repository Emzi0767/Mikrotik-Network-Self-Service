import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

import { AnyForm } from "../types/form.type";
import { KeyOf } from "../types/key-of.type";

export function requireIf<T extends AnyForm<T>>(field: KeyOf<T>): ValidatorFn {
  return (ctrl: AbstractControl): ValidationErrors | null => {
    const other = ctrl.parent?.get(field as string);
    if (other === undefined || other === null)
      return null;

    if (other.value === true && (ctrl.value === undefined || ctrl.value === null || (typeof ctrl.value === 'string' && ctrl.value.length === 0)))
      return { required: true };

    return null;
  };
}
