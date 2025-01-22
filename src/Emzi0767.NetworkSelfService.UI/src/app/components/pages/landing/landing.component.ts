import { Component } from '@angular/core';
import { CoreModule } from '../../../core.module';

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

}
