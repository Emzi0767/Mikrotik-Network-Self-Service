import { Routes } from '@angular/router';

import { NotFoundComponent } from './components/pages/not-found/not-found.component';
import { LandingComponent } from './components/pages/landing/landing.component';
import { LoginComponent } from './components/pages/login/login.component';

import { authenticationGuard } from './guards/authentication.guard';
import { anonymousGuard } from './guards/anonymous.guard';

export const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent,
    canActivate: [
      anonymousGuard,
    ],
  },

  {
    path: '',
    pathMatch: 'full',
    component: LandingComponent,
    canActivate: [
      authenticationGuard,
    ],
  },

  {
    path: '**',
    component: NotFoundComponent,
  },
];
