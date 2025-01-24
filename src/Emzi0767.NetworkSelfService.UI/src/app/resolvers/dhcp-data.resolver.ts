import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';

import { DhcpClientService } from '../services/grpc/dhcp-client.service';
import { DhcpInfoResponse } from '../proto/dhcp.pb';

export const dhcpDataResolver: ResolveFn<DhcpInfoResponse> = (route, state) => {
  const client = inject(DhcpClientService);
  return client.getInformation();
};
