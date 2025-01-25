import { Injectable } from "@angular/core";
import { Observable, tap } from "rxjs";

import { GrpcEvent, GrpcMessage, GrpcRequest } from "@ngx-grpc/common";
import { GrpcHandler, GrpcInterceptor } from "@ngx-grpc/core";

import { LoadingStateProviderService } from "../services/loading-state-provider.service";

@Injectable()
export class LoadingDetectorInterceptor implements GrpcInterceptor {

  constructor(
    private loadingStateProvider: LoadingStateProviderService,
  ) { }

  intercept<TRequest extends GrpcMessage, TResponse extends GrpcMessage>(request: GrpcRequest<TRequest, TResponse>, next: GrpcHandler): Observable<GrpcEvent<TResponse>> {
    this.loadingStateProvider.beginRequest();

    return next.handle(request)
      .pipe(tap(_ => this.loadingStateProvider.endRequest()));
  }
}
