import { NullableOptional } from "./nullable.type";

export interface IMetaTableEntry {
  property: string;
  value:    NullableOptional<string>;
};
