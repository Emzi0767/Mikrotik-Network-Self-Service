import { Component, HostBinding, Signal } from "@angular/core";
import { BreakpointObserver, Breakpoints } from "@angular/cdk/layout";
import { toSignal } from "@angular/core/rxjs-interop";
import { map } from "rxjs";

import { ThemeTypeProviderService } from "./services/theme-type-provider.service";
import { AuthenticationProviderService } from "./services/authentication-provider.service";
import { CoreModule } from "./core.module";

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

  public isTinyDisplay: Signal<boolean>;

  constructor(
    private themeTypeProvider: ThemeTypeProviderService,
    private breakpointObserver: BreakpointObserver,
    private authentication: AuthenticationProviderService,
  ) {
    this.isTinyDisplay = toSignal(
      this.breakpointObserver.observe([ Breakpoints.Small, Breakpoints.XSmall ])
        .pipe(map(x => x.matches)),
      { initialValue: false }
    );
  }

  toggleDarkMode(): void {
    this.themeTypeProvider.prefersDarkTheme.set(
      !this.themeTypeProvider.prefersDarkTheme()
    );
  }
}
