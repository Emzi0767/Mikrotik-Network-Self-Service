import { FormControl } from "@angular/forms";

import { Nullable } from "./nullable";

export interface ILoginForm {
  username: FormControl<Nullable<string>>;
  password: FormControl<Nullable<string>>;
  remember: FormControl<Nullable<boolean>>;
};
