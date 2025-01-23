import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { AuthenticationProviderService } from '../../../services/authentication-provider.service';
import { AuthenticationClientService } from '../../../services/grpc/authentication-client.service';
import { CoreModule } from '../../../core.module';

@Component({
  selector: 'app-logout',
  standalone: true,
  imports: [
    CoreModule,
  ],
  template: '',
  styles: ''
})
export class LogoutComponent implements OnInit {
  constructor(
    private router: Router,
    private authentication: AuthenticationProviderService,
    private authenticationClient: AuthenticationClientService,
  ) { }

  ngOnInit(): void {
    this.authenticationClient.logout(this.authentication.authenticationToken!)
      .subscribe({
        next: _ => this.complete(),
        error: _ => this.complete(),
      });
  }

  private complete(): void {
    this.authentication.clearSession();
    this.router.navigate([ '/' ]);
  }
}
