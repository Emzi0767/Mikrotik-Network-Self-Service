import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

import { MATERIAL_IMPORTS } from '../../../common-imports';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [
    RouterModule,
    ...MATERIAL_IMPORTS,
  ],
  templateUrl: './not-found.template.html',
  styles: ``
})
export class NotFoundComponent {

}
