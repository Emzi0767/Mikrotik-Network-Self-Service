import { Inject, Injectable, Signal, signal, WritableSignal } from '@angular/core';
import { LOCAL_STORAGE, StorageService } from './storage.service';
import { AuthenticationState } from '../types/authentication-state.enum';
import { AuthenticationResponse } from '../proto/auth.pb';

interface IAuthenticationData {
  authenticationToken: string;
  refreshToken: string;
  refreshDeadline: Date;
  validityDeadline: Date;
}

const KEY_AUTH_DATA = "arctis2.auth";

@Injectable({
  providedIn: 'root'
})
export class AuthenticationProviderService {
  private _data: IAuthenticationData | null = null;
  private _isAuthenticated: WritableSignal<boolean> = signal(false);

  get isAuthenticated(): Signal<boolean> {
    return this._isAuthenticated.asReadonly();
  }

  get authenticationToken(): string | null {
    if (this._data === null)
      return null;

    return this._data.authenticationToken;
  }

  get refreshToken(): string | null {
    if (this._data === null)
      return null;

    return this._data.refreshToken;
  }

  constructor(
    private storage: StorageService
  ) {
    this._data = this.loadData();
    this._isAuthenticated.set(this.getSessionState() !== AuthenticationState.UNAUTHENTICATED);
  }

  getSessionState(): AuthenticationState {
    const date = new Date();
    if (this._data === null)
      return AuthenticationState.UNAUTHENTICATED;

    const time = date.getTime();
    if (time >= this._data.validityDeadline.getTime()) {
      this._data = null;
      return AuthenticationState.UNAUTHENTICATED;
    }

    if (time >= this._data.refreshDeadline.getTime())
      return AuthenticationState.REQUIRES_REFRESH;

    return AuthenticationState.OK;
  }

  updateSession(session: AuthenticationResponse.AuthenticationSuccess): void {
    if (this._data === null) {
      if (session.expiresAt === undefined || session.refreshBy === undefined || session.refresh === undefined)
        return;

      this._data = {
        authenticationToken: session.token,
        refreshToken: session.refresh,
        refreshDeadline: session.refreshBy.toDate(),
        validityDeadline: session.expiresAt.toDate(),
      };
    } else {
      this._data.authenticationToken = session.token;

      if (session.refreshBy !== undefined)
        this._data.refreshDeadline = session.refreshBy.toDate();

      if (session.expiresAt !== undefined)
        this._data.validityDeadline = session.expiresAt.toDate();
    }

    this._isAuthenticated.set(this.getSessionState() !== AuthenticationState.UNAUTHENTICATED);
    this.saveData();
  }

  clearSession(): void {
    this._data = null;
    this._isAuthenticated.set(false);
    this.saveData();
  }

  private loadData(): IAuthenticationData | null {
    return this.storage.get<IAuthenticationData>(KEY_AUTH_DATA);
  }

  private saveData(): void {
    if (this._data === null)
      this.storage.remove(KEY_AUTH_DATA);
    else
      this.storage.set<IAuthenticationData>(KEY_AUTH_DATA, this._data);
  }
}
