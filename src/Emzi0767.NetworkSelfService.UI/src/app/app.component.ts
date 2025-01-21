import { Component, HostBinding, Signal } from "@angular/core";
import { BreakpointObserver, Breakpoints } from "@angular/cdk/layout";
import { map, tap } from "rxjs";
import { toSignal } from "@angular/core/rxjs-interop";
import { RouterOutlet } from "@angular/router";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";

import { MATERIAL_IMPORTS } from "./common-imports";
import { ThemeTypeProviderService } from "./services/theme-type-provider.service";
import { GrpcModule } from "./grpc.module";
import { AuthenticationProviderService } from "./services/authentication-provider.service";

@Component({
  selector: "app-root",
  standalone: true,
  imports: [
    RouterOutlet,
    CommonModule,
    FormsModule,
    GrpcModule,
    ...MATERIAL_IMPORTS,
  ],
  providers: [
    GrpcModule,
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
