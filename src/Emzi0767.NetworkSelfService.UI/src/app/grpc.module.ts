import { importProvidersFrom, NgModule } from "@angular/core";
import { GrpcCoreModule } from "@ngx-grpc/core";
import { GrpcWebClientModule } from "@ngx-grpc/grpc-web-client";

import { environment } from "../environments/environment";

const MODULE_CORE = GrpcCoreModule.forRoot();
const MODULE_WEB_CLIENT = GrpcWebClientModule.forRoot({
  settings: {
    format: "binary",
    host: environment.grpcHost,
  },
});

@NgModule({
  imports: [
    MODULE_CORE,
    MODULE_WEB_CLIENT,
  ],
  providers: [
    importProvidersFrom(MODULE_CORE),
    importProvidersFrom(MODULE_WEB_CLIENT),
  ]
})
export class GrpcModule { }
