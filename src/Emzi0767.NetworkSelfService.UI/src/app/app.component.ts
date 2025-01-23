import { Component, HostBinding, signal, Signal, WritableSignal } from "@angular/core";
import { ActivatedRoute, NavigationEnd, Router } from "@angular/router";
import { BreakpointObserver, Breakpoints } from "@angular/cdk/layout";
import { toSignal } from "@angular/core/rxjs-interop";
import { filter, map, switchMap } from "rxjs";

import { ThemeTypeProviderService } from "./services/theme-type-provider.service";
import { AuthenticationProviderService } from "./services/authentication-provider.service";
import { CoreModule } from "./core.module";
import { RouteCategory } from "./types/route-category.enum";

@Component({
  selector: "app-root",
  standalone: true,
  imports: [
    CoreModule,
  ],
  templateUrl: "app.template.html",
  styleUrl: "app.component.scss",
})
export class AppComponent {

  @HostBinding("class.dark-theme")
  get useDarkTheme(): boolean {
    return this.themeTypeProvider.prefersDarkTheme();
  }

  @HostBinding("class.light-theme")
  get useLightTheme(): boolean {
    return !this.themeTypeProvider.prefersDarkTheme();
  }

  get prefersDarkTheme(): Signal<boolean> {
    return this.themeTypeProvider.prefersDarkTheme;
  }

  get isAuthenticated(): Signal<boolean> {
    return this.authentication.isAuthenticated;
  }

  get currentRouteCategory(): Signal<RouteCategory> {
    return this._currentRouteCategory.asReadonly();
  }

  isTinyDisplay: Signal<boolean>;
  RouteCategory = RouteCategory;

  private _currentRouteCategory: WritableSignal<RouteCategory> = signal(RouteCategory.LANDING);

  constructor(
    private themeTypeProvider: ThemeTypeProviderService,
    private breakpointObserver: BreakpointObserver,
    private authentication: AuthenticationProviderService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
  ) {
    this.isTinyDisplay = toSignal(
      this.breakpointObserver.observe([ Breakpoints.Small, Breakpoints.XSmall ])
        .pipe(map(x => x.matches)),
      { initialValue: false }
    );

    this.router.events.pipe(
      filter(e => e instanceof NavigationEnd),
      map(() => this.activatedRoute),
      map(r => r.firstChild!),
      switchMap(r => r.data),
      map(d => d["category"] as RouteCategory)
    ).subscribe(x => this._currentRouteCategory.set(x));
  }

  toggleDarkMode(): void {
    this.themeTypeProvider.prefersDarkTheme.set(
      !this.themeTypeProvider.prefersDarkTheme()
    );
  }
}
