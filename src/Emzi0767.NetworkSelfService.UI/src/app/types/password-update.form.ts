import { NullableProperties } from "./nullable.type";
import { FormFor } from "./form.type";

export interface IPasswordUpdate {
  current: string;
  update:  string;
  confirm: string;
}

export type IPasswordUpdateData = NullableProperties<IPasswordUpdate>;
export type IPasswordUpdateForm = FormFor<IPasswordUpdateData>;
