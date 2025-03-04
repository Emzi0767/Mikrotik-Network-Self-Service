import { Component, Renderer2, signal, Signal, ViewChild, WritableSignal } from "@angular/core";
import { ActivatedRoute, NavigationEnd, Router } from "@angular/router";
import { BreakpointObserver, Breakpoints } from "@angular/cdk/layout";
import { toObservable } from "@angular/core/rxjs-interop";
import { filter, map, startWith, switchMap } from "rxjs";

import { ThemeType, ThemeTypeProviderService } from "./services/theme-type-provider.service";
import { AuthenticationProviderService } from "./services/authentication-provider.service";
import { CoreModule } from "./core.module";
import { RouteCategory } from "./types/route-category.enum";
import { LoadingStateProviderService } from "./services/loading-state-provider.service";
import { MatDrawer } from "@angular/material/sidenav";

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

  get prefersDarkTheme(): Signal<boolean> {
    return this.themeTypeProvider.prefersDarkTheme;
  }

  get isAuthenticated(): Signal<boolean> {
    return this.authentication.isAuthenticated;
  }

  get currentRouteCategory(): Signal<RouteCategory> {
    return this._currentRouteCategory.asReadonly();
  }

  get isTinyDisplay(): Signal<boolean> {
    return this._isTinyDisplay.asReadonly();
  }

  get isLoading(): Signal<boolean> {
    return this.loadingStateProvider.isLoading;
  }

  RouteCategory = RouteCategory;

  private _currentRouteCategory: WritableSignal<RouteCategory> = signal(RouteCategory.LANDING);
  private _isTinyDisplay: WritableSignal<boolean> = signal(false);

  @ViewChild("drawer") drawer?: MatDrawer;

  constructor(
    private themeTypeProvider: ThemeTypeProviderService,
    private breakpointObserver: BreakpointObserver,
    private authentication: AuthenticationProviderService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private renderer: Renderer2,
    private loadingStateProvider: LoadingStateProviderService,
  ) {
    this.breakpointObserver.observe([ Breakpoints.Small, Breakpoints.XSmall ])
        .pipe(
          map(x => x.matches),
          startWith(false),
        )
        .subscribe(x => {
          if (this.drawer !== undefined)
            this.drawer.close();

          this._isTinyDisplay.set(x);
          if (x)
            this.renderer.addClass(document.body, "tiny-display");
          else
            this.renderer.removeClass(document.body, "tiny-display");
        })

    this.router.events.pipe(
      filter(e => e instanceof NavigationEnd),
      map(() => this.activatedRoute),
      map(r => r.firstChild!),
      switchMap(r => r.data),
      map(d => d["category"] as RouteCategory)
    ).subscribe(x => this._currentRouteCategory.set(x));

    toObservable(this.prefersDarkTheme)
      .subscribe(x => {
        if (x) {
          this.renderer.addClass(document.body, "dark-theme");
          this.renderer.removeClass(document.body, "light-theme");
        } else {
          this.renderer.removeClass(document.body, "dark-theme");
          this.renderer.addClass(document.body, "light-theme");
        }
      });
  }

  toggleDarkMode(): void {
    this.themeTypeProvider.setPreferredTheme(
      this.themeTypeProvider.prefersDarkTheme()
        ? ThemeType.LIGHT
        : ThemeType.DARK
    );
  }
}
