import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { BehaviorSubject, Observable } from 'rxjs';

import { CoreModule } from '../../../core.module';
import { WifiAcl, WifiBand, WifiConnectedDevice, WifiInfoResponse, WifiRecentAttemptsResponse } from '../../../proto/wifi.pb';
import { IMetaTableEntry } from '../../../types/meta-table-entry.interface';
import { WifiClient } from '../../../proto/wifi.pbsc';
import { SnackService } from '../../../services/snack.service';

@Component({
  selector: 'app-wifi',
  imports: [
    CoreModule,
  ],
  templateUrl: './wifi.template.html',
  styleUrls: [
    './wifi.style.scss',
  ]
})
export class WifiComponent {
  information!: WifiInfoResponse;
  WifiBand = WifiBand;

  private _config$: BehaviorSubject<IMetaTableEntry[]> = new BehaviorSubject<IMetaTableEntry[]>([]);
  private _acls$: BehaviorSubject<WifiAcl[]> = new BehaviorSubject<WifiAcl[]>([]);
  private _connected$: BehaviorSubject<WifiConnectedDevice[]> = new BehaviorSubject<WifiConnectedDevice[]>([]);
  private _recents$: BehaviorSubject<string[]> = new BehaviorSubject<string[]>([]);

  get acls$(): Observable<WifiAcl[]> {
    return this._acls$.asObservable();
  }

  get config$(): Observable<IMetaTableEntry[]> {
    return this._config$.asObservable();
  }

  get connected$(): Observable<WifiConnectedDevice[]> {
    return this._connected$.asObservable();
  }

  get recents$(): Observable<string[]> {
    return this._recents$.asObservable();
  }

  constructor(
    private activatedRoute: ActivatedRoute,
    private wifiClient: WifiClient,
    private dialog: MatDialog,
    private snackService: SnackService,
  ) {
    this.activatedRoute.data.subscribe(data => {
      this.information = data["information"];

      this.updateConfig();
      this._acls$.next(this.information.accessControl?.acls!);
      this._connected$.next(this.information.connectedDevices?.devices!);
      this._recents$.next(this.information.recentAttempts?.macAddresses!);
    });
  }

  editConfig(): void {

  }

  createAcl(address?: string): void {

  }

  editAcl(acl: WifiAcl): void {

  }

  deleteAcl(acl: WifiAcl): void {

  }

  private updateConfig(): void {
    const config = [
      {
        property: "SSID",
        value: this.information.configuration?.ssid,
      },
      {
        property: "Client isolation",
        value: this.information.configuration?.isolateClients
          ? "Enabled"
          : "Disabled",
      },
    ];

    this._config$.next(config);
  }
}
