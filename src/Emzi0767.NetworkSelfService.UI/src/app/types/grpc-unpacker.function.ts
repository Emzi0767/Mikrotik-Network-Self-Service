import { Observable, of, throwError } from "rxjs";
import { GrpcMessage, GrpcMessagePool } from "@ngx-grpc/common";

import { ErrorCode, Result, Error } from "../proto/result.pb";

export function makeExtractor<T extends GrpcMessage>(pool: GrpcMessagePool): (result: Result) => Observable<T> {
  return (extractOrThrow<T>).bind(null, pool);
}

export function extractOrThrow<T extends GrpcMessage>(pool: GrpcMessagePool, result: Result): Observable<T> {
  if (result.isSuccess && result.result !== undefined)
    return of(result.result.unpack<T>(pool));

  if (result.error !== undefined)
    return throwError(() => result.error);

  return throwError(() => new Error({ code: ErrorCode.UNKNOWN }));
}

export function extractNullOrThrow(result: Result): Observable<null> {
  if (result.isSuccess)
    return of(null);

  if (result.error !== undefined)
    return throwError(() => result.error);

  return throwError(() => new Error({ code: ErrorCode.UNKNOWN }));
}
