import { Inject, Injectable, InjectionToken } from '@angular/core';

export const LOCAL_STORAGE = new InjectionToken<Storage>(
  "Local Storage",
  {
    providedIn: "root",
    factory: () => localStorage,
  }
)

@Injectable({
  providedIn: "root"
})
export class StorageService {

  constructor(@Inject(LOCAL_STORAGE) private storage: Storage) { }

  set<T>(key: string, value: T): void {
    this.storage.setItem(
      key,
      JSON.stringify(value)
    );
  }

  get<T>(key: string): T | null {
    const raw = this.storage.getItem(key);
    if (raw === null)
      return null;

    return JSON.parse(raw) as T;
  }

  remove(key: string): void {
    this.storage.removeItem(key);
  }

  clear(): void {
    this.storage.clear();
  }
}
