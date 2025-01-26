import { FormControl, FormGroup } from "@angular/forms";

import { AnyObject } from "./any-object.type";
import { KeyOf } from "./key-of.type";

export type FormFor<T extends AnyObject> = {
  [k in KeyOf<T>]: NonNullable<T[k]> extends AnyObject
    ? (NonNullable<T[k]> extends Date
      ? FormControl<T[k]>
      : FormGroup<FormFor<NonNullable<T[k]>>>)
    : FormControl<T[k]>;
};

export type AnyForm<T extends AnyObject> = {
  [k in KeyOf<T>]: T[k] extends FormControl<infer P>
    ? FormControl<P>
    : (T[k] extends FormGroup<FormFor<NonNullable<infer Q>>>
      ? FormGroup<FormFor<NonNullable<Q>>>
      : never);
};
