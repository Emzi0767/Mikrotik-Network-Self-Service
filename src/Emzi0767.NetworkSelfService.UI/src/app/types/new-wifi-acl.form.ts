import { WifiWeekday } from "../proto/wifi.pb";
import { FormFor } from "./form.type";
import { NullableProperties, Optional } from "./nullable.type";

export type WifiWeekdaySelector = {
  -readonly [k in keyof typeof WifiWeekday as k extends string ? k : never]: boolean;
};

export interface INewWifiAcl {
  macAddress:          string;
  comment:             string;
  hasPrivatePassword:  boolean;
  privatePassword:     Optional<string>;
  hasSchedule:         boolean;
  scheduleStart:       Optional<Date>;
  scheduleEnd:         Optional<Date>;
  weekdays:            NullableProperties<WifiWeekdaySelector>;
  enable:              boolean;
}

export type INewWifiAclData = NullableProperties<INewWifiAcl>;
export type INewWifiAclForm = FormFor<INewWifiAclData>;
