import { NgModule } from "@angular/core";
import { GrpcCoreModule } from "@ngx-grpc/core";
import { GrpcWebClientModule } from "@ngx-grpc/grpc-web-client";
import { environment } from "../environments/environment";

@NgModule({
  imports: [
    GrpcCoreModule.forRoot(),
    GrpcWebClientModule.forRoot({
      settings: {
        format: "binary",
        host: environment.grpcHost,
      },
    }),
  ],
})
export class GrpcModule { }
