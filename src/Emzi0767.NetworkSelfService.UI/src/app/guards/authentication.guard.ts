import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';

import { AuthenticationProviderService } from '../services/authentication-provider.service';
import { AuthenticationState } from '../types/authentication-state.enum';

export const authenticationGuard: CanActivateFn = (route, state) => {
  const authProvider = inject(AuthenticationProviderService);
  const authState = authProvider.getSessionState();
  const router = inject(Router);
  if (authState === AuthenticationState.UNAUTHENTICATED) {
    router.navigate([ '/login' ]);
    return false;
  }

  return true;
};
