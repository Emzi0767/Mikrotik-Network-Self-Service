import { effect, Injectable, signal, WritableSignal } from '@angular/core';
import { StorageService } from './storage.service';
import { fromEvent, map, Observable, startWith } from 'rxjs';

enum InternalThemeType {
  DEFAULT,
  LIGHT,
  DARK,
};

const KEY_THEME_TYPE = "arctis2.theme.type";

type WindowStateData = {
  theme: InternalThemeType,
  schemeChange: Observable<ThemeType>,
};

export enum ThemeType {
  LIGHT,
  DARK,
};

// true -> dark
// false -> light

@Injectable({
  providedIn: 'root'
})
export class ThemeTypeProviderService {

  prefersDarkTheme: WritableSignal<boolean>;

  constructor(private storage: StorageService) {
    let pref = this.getSavedPreference();
    const { theme, schemeChange } = this.getWindowState();
    if (pref === InternalThemeType.DEFAULT)
      pref = theme;

    this.prefersDarkTheme = signal(
      this._t2b(this._i2t(pref))
    );

    effect(() => {
      this.setPreferredTheme(this._b2t(this.prefersDarkTheme()));
    });

    schemeChange.subscribe(t => this.prefersDarkTheme.set(this._t2b(t)));
  }

  setPreferredTheme(theme: ThemeType): void {
    this.storage.set(KEY_THEME_TYPE, theme === ThemeType.DARK);
  }

  private _b2t(v: boolean): ThemeType {
    return v
      ? ThemeType.DARK
      : ThemeType.LIGHT;
  }

  private _t2b(v: ThemeType): boolean {
    return v === ThemeType.DARK;
  }

  private _i2t(v: InternalThemeType): ThemeType {
    return v === InternalThemeType.DARK
      ? ThemeType.DARK
      : ThemeType.LIGHT;
  }

  private getWindowState(): WindowStateData {
    const prefersColorScheme = window.matchMedia("(prefers-color-scheme: dark)");
    const theme = prefersColorScheme.matches
      ? InternalThemeType.DARK
      : InternalThemeType.LIGHT;

    const schemeChange = fromEvent<MediaQueryList>(prefersColorScheme, "change")
      .pipe(
        startWith(prefersColorScheme),
        map((q: MediaQueryList) => q.matches),
        map(r => r ? ThemeType.DARK : ThemeType.LIGHT),
        map(t => {
          const saved = this.getSavedPreference();
          if (saved === InternalThemeType.DEFAULT)
            return t;

          return saved === InternalThemeType.DARK
            ? ThemeType.DARK
            : ThemeType.LIGHT
        })
      );

    return { theme, schemeChange };
  }

  private getSavedPreference(): InternalThemeType {
    const pref = this.storage.get<boolean>(KEY_THEME_TYPE);
    if (pref === null)
      return InternalThemeType.DEFAULT;

    return pref
      ? InternalThemeType.DARK
      : InternalThemeType.LIGHT;
  }
}
