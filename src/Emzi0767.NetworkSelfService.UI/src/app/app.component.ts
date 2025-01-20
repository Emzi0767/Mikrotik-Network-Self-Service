import { Component, HostBinding, Signal } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { MatToolbarModule } from "@angular/material/toolbar";
import { CommonModule } from "@angular/common";
import { MatSlideToggleModule } from "@angular/material/slide-toggle";
import { FormsModule } from "@angular/forms";

import { ThemeTypeProviderService } from "./services/theme-type-provider.service";
import { BreakpointObserver, Breakpoints } from "@angular/cdk/layout";
import { map } from "rxjs";
import { toSignal } from "@angular/core/rxjs-interop";

@Component({
  selector: "app-root",
  imports: [
    RouterOutlet,
    MatToolbarModule,
    MatSlideToggleModule,
    CommonModule,
    FormsModule,
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
      this.breakpointObserver.observe(Breakpoints.XSmall)
        .pipe(map(x => x.matches)),
      { initialValue: false }
    );
  }
}
