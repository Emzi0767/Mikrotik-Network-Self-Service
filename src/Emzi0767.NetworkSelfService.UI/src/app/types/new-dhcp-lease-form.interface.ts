import { FormFor } from "./form.type";
import { NullableProperties } from "./nullable.type";

export interface INewDhcpLease {
  macAddress: string;
  ipAddress:  string;
}

export type INewDhcpLeaseData = NullableProperties<INewDhcpLease>;
export type INewDhcpLeaseForm = FormFor<INewDhcpLeaseData>;
