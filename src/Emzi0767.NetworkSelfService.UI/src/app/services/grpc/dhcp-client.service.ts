import { Injectable } from '@angular/core';
import { catchError, Observable, of, switchMap, throwError } from 'rxjs';

import { GrpcMessagePool, GrpcMetadata } from '@ngx-grpc/common';
import { Empty } from '@ngx-grpc/well-known-types';

import { DhcpClient } from '../../proto/dhcp.pbsc';
import { AuthenticationProviderService } from '../authentication-provider.service';
import { DhcpAddressEligibilityQuery, DhcpAddressEligibilityResponse, DhcpInfoResponse, DhcpLeaseCreateRequest, DhcpLeaseDeleteRequest, DhcpLeases, DhcpLeasesResponse } from '../../proto/dhcp.pb';
import { makeExtractor } from '../../types/grpc-unpacker.function';
import { Error, ErrorCode, Result } from '../../proto/result.pb';

const EXTRACT_DHCP_INFO = makeExtractor<DhcpInfoResponse>(new GrpcMessagePool([ DhcpInfoResponse, Empty ]));
const EXTRACT_DHCP_LEASES = makeExtractor<DhcpLeases>(new GrpcMessagePool([ DhcpLeases, Empty ]));
const EXTRACT_DHCP_ELIGIBILITY = makeExtractor<DhcpAddressEligibilityResponse>(new GrpcMessagePool([ DhcpAddressEligibilityResponse, Empty ]));

@Injectable({
  providedIn: 'root'
})
export class DhcpClientService {

  constructor(
    private dhcp: DhcpClient,
    private authentication: AuthenticationProviderService,
  ) { }

  getInformation(): Observable<DhcpInfoResponse> {
    const resp = this.dhcp.getInfo(
      new Empty(),
      new GrpcMetadata({
        "Authorization": `Bearer ${this.authentication.authenticationToken}`
      })
    );

    return resp.pipe(
      catchError((err, caught) => throwError(() => new Error({ code: ErrorCode.UNKNOWN }))),
      switchMap(EXTRACT_DHCP_INFO),
    );
  }

  getLeases(): Observable<DhcpLeases> {
    const resp = this.dhcp.getLeases(
      new Empty(),
      new GrpcMetadata({
        "Authorization": `Bearer ${this.authentication.authenticationToken}`
      })
    );

    return resp.pipe(
      catchError((err, caught) => throwError(() => new Error({ code: ErrorCode.UNKNOWN }))),
      switchMap(EXTRACT_DHCP_LEASES),
    );
  }

  queryLeaseEligibility(query: DhcpAddressEligibilityQuery): Observable<DhcpAddressEligibilityResponse> {
    const resp = this.dhcp.queryLeaseEligibility(
      query,
      new GrpcMetadata({
        "Authorization": `Bearer ${this.authentication.authenticationToken}`
      })
    );

    return resp.pipe(
      catchError((err, caught) => throwError(() => new Error({ code: ErrorCode.UNKNOWN }))),
      switchMap(EXTRACT_DHCP_ELIGIBILITY),
    );
  }

  createLease(lease: DhcpLeaseCreateRequest): Observable<Result> {
    const resp = this.dhcp.createLease(
      lease,
      new GrpcMetadata({
        "Authorization": `Bearer ${this.authentication.authenticationToken}`
      })
    );

    return resp.pipe(
      switchMap(x => x.isSuccess ? of(x) : throwError(() => new Error({ code: ErrorCode.UNKNOWN }))),
      catchError((err, caught) => throwError(() => new Error({ code: ErrorCode.UNKNOWN }))),
    );
  }

  deleteLease(lease: DhcpLeaseDeleteRequest): Observable<Result> {
    const resp = this.dhcp.deleteLease(
      lease,
      new GrpcMetadata({
        "Authorization": `Bearer ${this.authentication.authenticationToken}`
      })
    );

    return resp.pipe(
      switchMap(x => x.isSuccess ? of(x) : throwError(() => new Error({ code: ErrorCode.UNKNOWN }))),
      catchError((err, caught) => throwError(() => new Error({ code: ErrorCode.UNKNOWN }))),
    );
  }
}
