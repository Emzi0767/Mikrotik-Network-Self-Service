import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { CoreModule } from '../../../core.module';
import { LandingResponse } from '../../../proto/landing.pb';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [
    CoreModule,
  ],
  templateUrl: './landing.template.html',
  styles: ``
})
export class LandingComponent {
  information!: LandingResponse;

  constructor(
    private activatedRoute: ActivatedRoute,
  ) {
    this.activatedRoute.data.subscribe(data => {
      this.information = data["information"];
    });
  }
}
