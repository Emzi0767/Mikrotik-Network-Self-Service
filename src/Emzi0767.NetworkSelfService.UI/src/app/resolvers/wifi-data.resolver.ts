import { ResolveFn } from '@angular/router';

export const wifiDataResolver: ResolveFn<boolean> = (route, state) => {
  return true;
};
