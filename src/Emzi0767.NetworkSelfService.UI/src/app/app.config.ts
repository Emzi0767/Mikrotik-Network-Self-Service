import { ApplicationConfig, importProvidersFrom, provideExperimentalZonelessChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';

import { GRPC_INTERCEPTORS, GrpcCoreModule } from '@ngx-grpc/core';
import { GrpcWebClientModule } from '@ngx-grpc/grpc-web-client';

import { routes } from './app.routes';
import { environment } from '../environments/environment';
import { GrpcAuthInterceptor } from './interceptors/grpc-auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideExperimentalZonelessChangeDetection(),
    provideRouter(routes),
    provideAnimationsAsync(),
    {
      provide: GRPC_INTERCEPTORS,
      useClass: GrpcAuthInterceptor,
      multi: true,
    },
    importProvidersFrom(GrpcCoreModule.forRoot()),
    importProvidersFrom(GrpcWebClientModule.forRoot({
      settings: {
        format: "binary",
        host: environment.grpcHost,
      },
    })),
  ],
};
