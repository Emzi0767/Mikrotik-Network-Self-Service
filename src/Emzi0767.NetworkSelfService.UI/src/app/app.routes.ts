import { Routes } from '@angular/router';

import { NotFoundComponent } from './components/pages/not-found/not-found.component';
import { LandingComponent } from './components/pages/landing/landing.component';
import { LoginComponent } from './components/pages/login/login.component';

import { authenticationGuard } from './guards/authentication.guard';
import { anonymousGuard } from './guards/anonymous.guard';
import { LogoutComponent } from './components/pages/logout/logout.component';

export const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent,
    runGuardsAndResolvers: 'always',
    canActivate: [
      anonymousGuard,
    ],
  },

  {
    path: 'logout',
    component: LogoutComponent,
    runGuardsAndResolvers: 'always',
    canActivate: [
      authenticationGuard,
    ],
  },

  {
    path: '',
    pathMatch: 'full',
    component: LandingComponent,
    runGuardsAndResolvers: 'always',
    canActivate: [
      authenticationGuard,
    ],
  },

  {
    path: '**',
    component: NotFoundComponent,
  },
];
