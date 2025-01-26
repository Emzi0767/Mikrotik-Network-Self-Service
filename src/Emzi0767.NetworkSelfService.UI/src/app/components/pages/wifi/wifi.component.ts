import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { BehaviorSubject, Observable } from 'rxjs';

import { CoreModule } from '../../../core.module';
import { WifiAcl, WifiAclCreateRequest, WifiAclDeleteRequest, WifiAclUpdateRequest, WifiBand, WifiConnectedDevice, WifiInfoResponse, WifiTimeRestriction, WifiUpdateRequest, WifiWeekday } from '../../../proto/wifi.pb';
import { IMetaTableEntry } from '../../../types/meta-table-entry.interface';
import { SnackService } from '../../../services/snack.service';
import { WifiEditConfigComponent } from '../../dialogs/wifi-edit-config/wifi-edit-config.component';
import { IWifiConfig } from '../../../types/wifi-config.form';
import { WifiClientService } from '../../../services/grpc/wifi-client.service';
import { WifiRemoveAclComponent } from '../../dialogs/wifi-remove-acl/wifi-remove-acl.component';
import { INewWifiAcl, INewWifiAclData } from '../../../types/new-wifi-acl.form';
import { WifiAddAclComponent } from '../../dialogs/wifi-add-acl/wifi-add-acl.component';
import { Duration } from '@ngx-grpc/well-known-types';

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
      if (config.ssid !== undefined && config.ssid !== null && config.ssid.length > 0 && config.ssid !== this.information.configuration?.ssid)
        newConfig.ssid = config.ssid;

      if (config.password !== undefined && config.password !== null && config.password.length > 0)
        newConfig.password = config.password;

      if (config.isolateClients !== undefined && config.isolateClients !== null && config.isolateClients !== this.information.configuration!.isolateClients)
        newConfig.isolateClients = config.isolateClients;

      this.wifiClient.updateConfiguration(new WifiUpdateRequest(newConfig))
        .subscribe({
          next: x => this.handleConfigUpdate(),
          error: _ => this.snackService.show("Could not update Wi-Fi configuration.", "Dismiss"),
        });
    });
  }

  createAcl(address?: string): void {
    const acl = address !== undefined
      ? new WifiAcl({ macAddress: address, isEnabled: true, })
      : null;

    this.editAcl(acl, true);
  }

  editAcl(acl: WifiAcl | null, create: boolean): void {
    const dialogRef = this.dialog.open(
      WifiAddAclComponent,
      { data: { acl, create, }, }
    );

    dialogRef.afterClosed().subscribe(res => {
      if (res === undefined || res === null)
        return;

      const aclData = res as INewWifiAclData;
      if (create) {
        const newAcl = {
          macAddress: aclData.macAddress,
          comment: aclData.comment,
        } as WifiAclCreateRequest.AsObject;

        if (aclData.hasPrivatePassword && aclData.privatePassword !== undefined && aclData.privatePassword !== null)
          newAcl.privatePassword = aclData.privatePassword;

        if (aclData.hasSchedule)
          newAcl.timeRestriction = this.makeSchedule(aclData);

        this.wifiClient.createAcl(new WifiAclCreateRequest(newAcl))
          .subscribe({
            next: x => this.reloadAcls(),
            error: _ => this.snackService.show("Could not create whitelist entry", "Dismiss"),
          });
      } else {
        const modAcl = {
          identifier: acl!.id,
          macAddress: aclData.macAddress,
          comment: aclData.comment,
          isEnabled: !!aclData.enable,
        } as WifiAclUpdateRequest.AsObject;

        if (acl!.privatePassword !== undefined && acl!.privatePassword !== null && !aclData.hasPrivatePassword)
          modAcl.removePrivatePassword = true;

        if (aclData.hasPrivatePassword && aclData.privatePassword !== undefined && aclData.privatePassword !== null)
          modAcl.privatePassword = aclData.privatePassword;

        if (acl!.timeRestriction !== undefined && acl!.privatePassword !== null && !aclData.hasSchedule)
          modAcl.removeTimeRestriction = true;

        if (aclData.hasSchedule)
          modAcl.timeRestriction = this.makeSchedule(aclData);

        this.wifiClient.updateAcl(new WifiAclUpdateRequest(modAcl))
          .subscribe({
            next: x => this.reloadAcls(),
            error: _ => this.snackService.show("Could not update whitelist entry", "Dismiss"),
          });
      }
    });
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

  private makeSchedule(aclData: INewWifiAclData): WifiTimeRestriction {
    const [ sh, sm, ss ] = [ aclData.scheduleStart!.getHours(), aclData.scheduleStart!.getMinutes(), aclData.scheduleStart!.getSeconds(), ];
    const [ eh, em, es ] = [ aclData.scheduleEnd!.getHours(), aclData.scheduleEnd!.getMinutes(), aclData.scheduleEnd!.getSeconds(), ];

    const start = new Duration({ seconds: (sh * 3600 + sm * 60 + ss).toString(), });
    const end = new Duration({ seconds: (eh * 3600 + em * 60 + es).toString(), });
    const weekdays = [];
    for (const k of Object.keys(aclData.weekdays!)) {
      const key = k as keyof typeof WifiWeekday;
      if (key === 'UNKNOWN')
        continue;

      if (aclData.weekdays![key])
        weekdays.push(WifiWeekday[key]);
    }

    return new WifiTimeRestriction({
      start,
      end,
      days: weekdays,
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
