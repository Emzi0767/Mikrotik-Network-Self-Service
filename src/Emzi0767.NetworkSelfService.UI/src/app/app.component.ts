import { Component, HostBinding, Signal } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { BreakpointObserver, Breakpoints } from "@angular/cdk/layout";
import { map, tap } from "rxjs";
import { toSignal } from "@angular/core/rxjs-interop";

import { MATERIAL_IMPORTS } from "./common-imports";
import { ThemeType, ThemeTypeProviderService } from "./services/theme-type-provider.service";
import { GrpcModule } from "./grpc.module";

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

  public isTinyDisplay: Signal<boolean>;

  constructor(
    public themeTypeProvider: ThemeTypeProviderService,
    public breakpointObserver: BreakpointObserver
  ) {
    this.isTinyDisplay = toSignal(
      this.breakpointObserver.observe([ Breakpoints.Small, Breakpoints.XSmall ])
        .pipe(tap(console.log), map(x => x.matches)),
      { initialValue: false }
    );
  }

  toggleDarkMode(): void {
    this.themeTypeProvider.prefersDarkTheme.set(
      !this.themeTypeProvider.prefersDarkTheme()
    );
  }
}
