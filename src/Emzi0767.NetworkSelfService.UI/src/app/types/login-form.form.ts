import { NullableProperties } from "./nullable.type";
import { FormFor } from "./form.type";

export interface ILogin {
  username: string;
  password: string;
  remember: boolean;
}

export type ILoginData = NullableProperties<ILogin>;
export type ILoginForm = FormFor<ILoginData>;
