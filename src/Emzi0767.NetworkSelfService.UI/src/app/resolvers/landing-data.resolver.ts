import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';

import { LandingClientService } from '../services/grpc/landing-client.service';
import { LandingResponse } from '../proto/landing.pb';

export const landingDataResolver: ResolveFn<LandingResponse> = (route, state) => {
  const client = inject(LandingClientService);
  return client.getInformation();
};
