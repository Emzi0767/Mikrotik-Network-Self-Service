import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { BehaviorSubject, Observable } from 'rxjs';

import { CoreModule } from '../../../core.module';
import { WifiAcl, WifiAclDeleteRequest, WifiBand, WifiConnectedDevice, WifiInfoResponse, WifiUpdateRequest } from '../../../proto/wifi.pb';
import { IMetaTableEntry } from '../../../types/meta-table-entry.interface';
import { SnackService } from '../../../services/snack.service';
import { WifiEditConfigComponent } from '../../dialogs/wifi-edit-config/wifi-edit-config.component';
import { IWifiConfig } from '../../../types/wifi-config.form';
import { WifiClientService } from '../../../services/grpc/wifi-client.service';
import { WifiRemoveAclComponent } from '../../dialogs/wifi-remove-acl/wifi-remove-acl.component';

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
    private wifiClient: WifiClientService,
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
    const dialogRef = this.dialog.open(
      WifiEditConfigComponent,
      { data: this.information.configuration, }
    );

    dialogRef.afterClosed().subscribe(res => {
      if (res === null || res === undefined)
        return;

      const config = res as IWifiConfig;
      const newConfig = {} as IWifiConfig;
      if (config.ssid !== this.information.configuration?.ssid)
        newConfig.ssid = config.ssid;

      if (config.password !== undefined && config.password !== null)
        newConfig.password = config.password;

      if (config.isolateClients !== this.information.configuration!.isolateClients)
        newConfig.isolateClients = config.isolateClients;

      console.log(newConfig);
      this.wifiClient.updateConfiguration(new WifiUpdateRequest(newConfig))
        .subscribe({
          next: x => this.handleConfigUpdate(),
          error: _ => this.snackService.show("Could not update Wi-Fi configuration.", "Dismiss"),
        });
    });
  }

  createAcl(address?: string): void {

  }

  editAcl(acl: WifiAcl): void {

  }

  deleteAcl(acl: WifiAcl): void {
    const dialogRef = this.dialog.open(
      WifiRemoveAclComponent
    );

    dialogRef.afterClosed().subscribe(res => {
      if (!res)
        return;

      this.wifiClient.deleteAcl(new WifiAclDeleteRequest({ identifier: acl.id, }))
        .subscribe({
          next: _ => this.reloadAcls(),
          error: _ => this.snackService.show("Could not delete whitelist entry", "Dismiss"),
        });
    });
  }

  private reloadAcls(): void {
    this.wifiClient.getAcls()
      .subscribe({
        next: x => {
          this.information.accessControl = x;
          this._acls$.next(this.information.accessControl.acls!);
        },
        error: _ => this.snackService.show("Could not reload whitelist.", "Dismiss"),
      });
  }

  private handleConfigUpdate(): void {
    this.wifiClient.getConfiguration()
      .subscribe({
        next: x => {
          this.information.configuration = x;
          this.updateConfig();
        },
        error: _ => this.snackService.show("Could not reload Wi-Fi configuration.", "Dismiss"),
      });
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
