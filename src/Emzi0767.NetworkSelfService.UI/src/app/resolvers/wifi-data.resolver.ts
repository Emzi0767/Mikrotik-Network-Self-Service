import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';

import { WifiInfoResponse } from '../proto/wifi.pb';
import { WifiClientService } from '../services/grpc/wifi-client.service';

export const wifiDataResolver: ResolveFn<WifiInfoResponse> = (route, state) => {
  const client = inject(WifiClientService);
  return client.getInformation();
};
