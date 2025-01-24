import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { catchError, map, of, tap } from 'rxjs';

import { AuthenticationProviderService } from '../services/authentication-provider.service';
import { AuthenticationState } from '../types/authentication-state.enum';
import { AuthenticationClientService } from '../services/grpc/authentication-client.service';

export const authenticationGuard: CanActivateFn = (route, state) => {
  const authProvider = inject(AuthenticationProviderService);
  const authState = authProvider.getSessionState();
  const router = inject(Router);
  if (authState === AuthenticationState.OK) {
    return true;
  } else if (authState === AuthenticationState.UNAUTHENTICATED) {
    router.navigate([ '/login' ]);
    return false;
  }

  const authClient = inject(AuthenticationClientService);
  return authClient.refreshSession(authProvider.refreshToken!)
    .pipe(
      tap(x => {
        if (x.session !== null && x.session !== undefined)
          authProvider.updateSession(x.session);
      }),
      map(x => true),
      catchError((err, caught) => of(false)),
    );
};
