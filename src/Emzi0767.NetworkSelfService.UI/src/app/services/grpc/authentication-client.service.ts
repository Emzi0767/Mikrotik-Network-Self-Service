import { Injectable } from '@angular/core';
import { catchError, Observable, of, switchMap, throwError } from 'rxjs';
import { GrpcMessagePool, GrpcMetadata } from '@ngx-grpc/common';

import { AuthenticationClient } from '../../proto/auth.pbsc';
import { AuthenticationRequest, AuthenticationResponse, PasswordUpdateRequest } from '../../proto/auth.pb';
import { Error, ErrorCode, Result } from '../../proto/result.pb';
import { Empty } from '@ngx-grpc/well-known-types';
import { extractNullOrThrow, makeExtractor } from '../../types/grpc-unpacker.function';

const EXTRACT_SESSION = makeExtractor<AuthenticationResponse>(new GrpcMessagePool([ AuthenticationResponse, Empty ]));

@Injectable({
  providedIn: 'root'
})
export class AuthenticationClientService {

  constructor(
    private authentication: AuthenticationClient
  ) { }

  authenticate(username: string, password: string, remember: boolean): Observable<AuthenticationResponse> {
    const resp = this.authentication.authenticate(
      new AuthenticationRequest({
        username,
        password,
        rememberMe: remember,
      })
    );

    return resp.pipe(
      catchError((err, caught) => throwError(() => new Error({ code: ErrorCode.UNKNOWN }))),
      switchMap(EXTRACT_SESSION),
    );
  }

  refreshSession(refreshToken: string): Observable<AuthenticationResponse> {
    const resp = this.authentication.refreshSession(
      new Empty(),
      new GrpcMetadata({ "Authorization": `Bearer ${refreshToken}` })
    );

    return resp.pipe(
      catchError((err, caught) => throwError(() => new Error({ code: ErrorCode.UNKNOWN }))),
      switchMap(EXTRACT_SESSION),
    );
  }

  logout(token: string): Observable<null> {
    const resp = this.authentication.destroySession(new Empty());

    return resp.pipe(
      catchError((err, caught) => throwError(() => new Error({ code: ErrorCode.UNKNOWN }))),
      switchMap(extractNullOrThrow),
    );
  }

  updatePassword(req: PasswordUpdateRequest): Observable<Result> {
    const resp = this.authentication.changePassword(req);

    return resp.pipe(
      switchMap(x => x.isSuccess ? of(x) : throwError(() => new Error({ code: ErrorCode.UNKNOWN }))),
      catchError((err, caught) => throwError(() => new Error({ code: ErrorCode.UNKNOWN }))),
    );
  }
}
