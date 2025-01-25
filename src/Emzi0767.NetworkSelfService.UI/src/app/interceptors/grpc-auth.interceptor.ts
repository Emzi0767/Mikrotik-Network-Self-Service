import { inject, Injectable } from "@angular/core";
import { catchError, concatMap, map, Observable, of, tap, throwError } from "rxjs";

import { GrpcMessage, GrpcRequest, GrpcEvent } from "@ngx-grpc/common";
import { GrpcHandler, GrpcInterceptor } from "@ngx-grpc/core";

import { AuthenticationClientService } from "../services/grpc/authentication-client.service";
import { AuthenticationProviderService } from "../services/authentication-provider.service";
import { AuthenticationState } from "../types/authentication-state.enum";
import { Error, ErrorCode } from "../proto/result.pb";

const NO_INTERCEPT_PATHS = [
  '/authentication.Authentication/Authenticate',
  '/authentication.Authentication/RefreshSession',
];

@Injectable()
export class GrpcAuthInterceptor implements GrpcInterceptor {

  constructor(
    private authentication: AuthenticationProviderService,
  ) { }

  intercept<TRequest extends GrpcMessage, TResponse extends GrpcMessage>(request: GrpcRequest<TRequest, TResponse>, next: GrpcHandler): Observable<GrpcEvent<TResponse>> {
    if (NO_INTERCEPT_PATHS.includes(request.path))
      return next.handle(request);

    const authState = this.authentication.getSessionState();
    if (authState === AuthenticationState.UNAUTHENTICATED) {
      return throwError(() => new Error({ code: ErrorCode.SESSION_INVALID_OR_EXPIRED, }));
    }

    if (authState === AuthenticationState.OK) {
      if (!request.requestMetadata.has("Authorization"))
        request.requestMetadata.set("Authorization", `Bearer ${this.authentication.authenticationToken}`);

      return next.handle(request);
    }

    const authenticationClient = inject(AuthenticationClientService);
    return authenticationClient.refreshSession(this.authentication.refreshToken!)
      .pipe(
        tap(x => {
          if (x.session !== null && x.session !== undefined)
            this.authentication.updateSession(x.session);
        }),
        map(x => true),
        catchError((err, caught) => of(false)),
        concatMap(x => {
          if (!x)
            return throwError(() => new Error({ code: ErrorCode.SESSION_INVALID_OR_EXPIRED, }));

          if (!request.requestMetadata.has("Authorization"))
            request.requestMetadata.set("Authorization", `Bearer ${this.authentication.authenticationToken}`);

          return next.handle(request);
        })
      );
  }
}
