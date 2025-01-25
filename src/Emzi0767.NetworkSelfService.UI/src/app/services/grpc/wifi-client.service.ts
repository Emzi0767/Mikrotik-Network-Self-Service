import { Injectable } from '@angular/core';
import { catchError, Observable, of, switchMap, throwError } from 'rxjs';

import { Empty } from '@ngx-grpc/well-known-types';
import { GrpcMessagePool } from '@ngx-grpc/common';

import { WifiClient } from '../../proto/wifi.pbsc';
import { WifiAclCreateRequest, WifiAclDeleteRequest, WifiAclResponse, WifiAclUpdateRequest, WifiConfigResponse, WifiInfoResponse, WifiRecentAttemptsResponse, WifiUpdateRequest } from '../../proto/wifi.pb';
import { makeExtractor } from '../../types/grpc-unpacker.function';
import { Error, ErrorCode, Result } from '../../proto/result.pb';

const EXTRACT_WIFI_INFO = makeExtractor<WifiInfoResponse>(new GrpcMessagePool([ WifiInfoResponse, Empty ]));
const EXTRACT_WIFI_CONFIG = makeExtractor<WifiConfigResponse>(new GrpcMessagePool([ WifiConfigResponse, Empty ]));
const EXTRACT_WIFI_ACL = makeExtractor<WifiAclResponse>(new GrpcMessagePool([ WifiAclResponse, Empty ]));
const EXTRACT_WIFI_RECENT = makeExtractor<WifiRecentAttemptsResponse>(new GrpcMessagePool([ WifiRecentAttemptsResponse, Empty ]));

@Injectable({
  providedIn: 'root'
})
export class WifiClientService {

  constructor(
    private wifi: WifiClient,
  ) { }

  getInformation(): Observable<WifiInfoResponse> {
    const resp = this.wifi.getInformation(new Empty());

    return resp.pipe(
      catchError((err, caught) => throwError(() => new Error({ code: ErrorCode.UNKNOWN }))),
      switchMap(EXTRACT_WIFI_INFO),
    );
  }

  getConfiguration(): Observable<WifiConfigResponse> {
    const resp = this.wifi.getConfiguration(new Empty());

    return resp.pipe(
      catchError((err, caught) => throwError(() => new Error({ code: ErrorCode.UNKNOWN }))),
      switchMap(EXTRACT_WIFI_CONFIG),
    );
  }

  getAcls(): Observable<WifiAclResponse> {
    const resp = this.wifi.getAcls(new Empty());

    return resp.pipe(
      catchError((err, caught) => throwError(() => new Error({ code: ErrorCode.UNKNOWN }))),
      switchMap(EXTRACT_WIFI_ACL),
    );
  }

  getRecentConnectionAttempts(): Observable<WifiRecentAttemptsResponse> {
    const resp = this.wifi.getRecentConnectionAttempts(new Empty());

    return resp.pipe(
      catchError((err, caught) => throwError(() => new Error({ code: ErrorCode.UNKNOWN }))),
      switchMap(EXTRACT_WIFI_RECENT),
    );
  }

  updateConfiguration(req: WifiUpdateRequest): Observable<Result> {
    const resp = this.wifi.updateConfiguration(req);

    return resp.pipe(
      switchMap(x => x.isSuccess ? of(x) : throwError(() => new Error({ code: ErrorCode.UNKNOWN }))),
      catchError((err, caught) => throwError(() => new Error({ code: ErrorCode.UNKNOWN }))),
    );
  }

  createAcl(req: WifiAclCreateRequest): Observable<Result> {
    const resp = this.wifi.createAcl(req);

    return resp.pipe(
      switchMap(x => x.isSuccess ? of(x) : throwError(() => new Error({ code: ErrorCode.UNKNOWN }))),
      catchError((err, caught) => throwError(() => new Error({ code: ErrorCode.UNKNOWN }))),
    );
  }

  deleteAcl(req: WifiAclDeleteRequest): Observable<Result> {
    const resp = this.wifi.deleteAcl(req);

    return resp.pipe(
      switchMap(x => x.isSuccess ? of(x) : throwError(() => new Error({ code: ErrorCode.UNKNOWN }))),
      catchError((err, caught) => throwError(() => new Error({ code: ErrorCode.UNKNOWN }))),
    );
  }

  updateAcl(req: WifiAclUpdateRequest): Observable<Result> {
    const resp = this.wifi.updateAcl(req);

    return resp.pipe(
      switchMap(x => x.isSuccess ? of(x) : throwError(() => new Error({ code: ErrorCode.UNKNOWN }))),
      catchError((err, caught) => throwError(() => new Error({ code: ErrorCode.UNKNOWN }))),
    );
  }
}
