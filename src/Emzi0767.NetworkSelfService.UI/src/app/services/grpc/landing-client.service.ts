import { Injectable } from '@angular/core';
import { catchError, Observable, switchMap, throwError } from 'rxjs';

import { Empty } from '@ngx-grpc/well-known-types';
import { GrpcMessagePool, GrpcMetadata } from '@ngx-grpc/common';

import { LandingResponse } from '../../proto/landing.pb';
import { makeExtractor } from '../../types/grpc-unpacker.function';
import { LandingClient } from '../../proto/landing.pbsc';
import { AuthenticationProviderService } from '../authentication-provider.service';
import { Error, ErrorCode } from '../../proto/result.pb';

const EXTRACT_LANDING_DATA = makeExtractor<LandingResponse>(new GrpcMessagePool([ LandingResponse, Empty ]));

@Injectable({
  providedIn: 'root'
})
export class LandingClientService {

  constructor(
    private landing: LandingClient,
    private authentication: AuthenticationProviderService,
  ) { }

  getInformation(): Observable<LandingResponse> {
    const resp = this.landing.getInformation(
      new Empty(),
      new GrpcMetadata({
        "Authorization": `Bearer ${this.authentication.authenticationToken}`
      })
    );

    return resp.pipe(
      catchError((err, caught) => throwError(() => new Error({ code: ErrorCode.UNKNOWN }))),
      switchMap(EXTRACT_LANDING_DATA),
    );
  }
}
