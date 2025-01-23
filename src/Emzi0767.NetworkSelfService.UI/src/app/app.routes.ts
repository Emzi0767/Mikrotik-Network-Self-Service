import { Routes } from '@angular/router';

import { NotFoundComponent } from './components/pages/not-found/not-found.component';
import { LandingComponent } from './components/pages/landing/landing.component';
import { LoginComponent } from './components/pages/login/login.component';

import { authenticationGuard } from './guards/authentication.guard';
import { anonymousGuard } from './guards/anonymous.guard';
import { LogoutComponent } from './components/pages/logout/logout.component';
import { landingDataResolver } from './resolvers/landing-data.resolver';
import { RouteCategory } from './types/route-category.enum';
import { UserSettingsComponent } from './components/pages/user-settings/user-settings.component';

export const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent,
    runGuardsAndResolvers: 'always',
    canActivate: [
      anonymousGuard,
    ],
    data: {
      category: RouteCategory.OTHER,
    },
  },

  {
    path: 'logout',
    component: LogoutComponent,
    runGuardsAndResolvers: 'always',
    canActivate: [
      authenticationGuard,
    ],
    data: {
      category: RouteCategory.OTHER,
    },
  },

  {
    path: 'user',
    component: UserSettingsComponent,
    runGuardsAndResolvers: 'always',
    canActivate: [
      authenticationGuard,
    ],
    data: {
      category: RouteCategory.USER_SETTINGS,
    },
  },

  {
    path: '',
    pathMatch: 'full',
    component: LandingComponent,
    runGuardsAndResolvers: 'always',
    canActivate: [
      authenticationGuard,
    ],
    resolve: {
      information: landingDataResolver,
    },
    data: {
      category: RouteCategory.LANDING,
    },
  },

  {
    path: '**',
    component: NotFoundComponent,
    data: {
      category: RouteCategory.OTHER,
    },
  },
];
