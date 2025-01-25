import { Injectable, Signal, signal, WritableSignal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoadingStateProviderService {

  get isLoading(): Signal<boolean> {
    return this._isLoading.asReadonly();
  }

  private _isLoading: WritableSignal<boolean> = signal(false);
  private _outstandingRequests = 0;

  constructor() { }

  beginRequest(): void {
    if (this._outstandingRequests++ <= 0)
      this._isLoading.set(true);
  }

  endRequest(): void {
    if (--this._outstandingRequests < 1)
      this._isLoading.set(false);

    if (this._outstandingRequests < 0)
      this._outstandingRequests = 0;
  }
}
