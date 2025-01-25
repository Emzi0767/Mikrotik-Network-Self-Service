import { FormFor } from "./form.type";
import { NullableProperties, Optional } from "./nullable.type";

export interface IWifiConfig {
  ssid:            Optional<string>;
  password:        Optional<string>;
  isolateClients:  Optional<boolean>;
}

export type IWifiConfigData = NullableProperties<IWifiConfig>;
export type IWifiConfigForm = FormFor<IWifiConfigData>;
