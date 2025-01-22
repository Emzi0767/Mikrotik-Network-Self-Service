import { Component } from '@angular/core';

import { CoreModule } from '../../../core.module';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [
    CoreModule,
  ],
  templateUrl: './not-found.template.html',
  styles: `h1, p { text-align: center; }`
})
export class NotFoundComponent { }
