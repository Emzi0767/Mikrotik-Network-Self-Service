import { FormArray, FormControl, FormGroup } from "@angular/forms";

import { AnyObject } from "./any-object.type";
import { KeyOf } from "./key-of.type";
import { Nullable } from "./nullable.type";

export type FormFor<T extends AnyObject> = {
  [k in KeyOf<T>]: T[k] extends { valueOf(): string }
    ? FormArray<FormControl<Nullable<boolean>>>
    : (T[k] extends AnyObject
      ? FormGroup<FormFor<T[k]>>
      : FormControl<T[k]>);
};

export type AnyForm<T extends AnyObject> = {
  [k in KeyOf<T>]: FormControl<T[k]> | FormArray<FormControl<Nullable<boolean>>> | FormGroup<FormFor<T[k]>>;
};
