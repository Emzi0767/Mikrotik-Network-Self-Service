import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthenticationProviderService } from '../services/authentication-provider.service';
import { AuthenticationState } from '../types/authentication-state.enum';

export const anonymousGuard: CanActivateFn = (route, state) => {
  const authProvider = inject(AuthenticationProviderService);
  const authState = authProvider.getSessionState();
  const router = inject(Router);
  if (authState === AuthenticationState.UNAUTHENTICATED)
    return true;

  router.navigate([ '/' ]);
  return false;
};
