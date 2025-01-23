import { FormControl } from "@angular/forms";

import { Nullable } from "./nullable";

export interface IPasswordUpdateForm {
  current:  FormControl<Nullable<string>>;
  update:   FormControl<Nullable<string>>;
  confirm:  FormControl<Nullable<string>>;
};
