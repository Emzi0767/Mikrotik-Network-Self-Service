import { KeyOf } from "./key-of.type";

export type Nullable<T> = T | null;
export type Optional<T> = T | undefined;
export type NullableOptional<T> = T | undefined | null;

export type NullableProperties<T> = {
  [k in KeyOf<T>]: Nullable<T[k]>;
};

export type OptionalProperties<T> = {
  [k in KeyOf<T>]: Nullable<T[k]>;
};

export type NullableOptionalProperties<T> = {
  [k in KeyOf<T>]: Nullable<T[k]>;
};
