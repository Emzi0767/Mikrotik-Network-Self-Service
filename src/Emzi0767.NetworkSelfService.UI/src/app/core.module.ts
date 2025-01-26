import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { MATERIAL_IMPORTS } from './common-imports';
import { provideNativeDateAdapter } from '@angular/material/core';

const CORE_MODULES = [
  CommonModule,
  RouterModule,
  FormsModule,
  ReactiveFormsModule,
  ...MATERIAL_IMPORTS,
];

@NgModule({
  declarations: [],
  imports: CORE_MODULES,
  exports: CORE_MODULES,
  providers: [
    provideNativeDateAdapter(),
  ],
})
export class CoreModule { }
