import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { BehaviorSubject, Observable } from 'rxjs';

import { CoreModule } from '../../../core.module';
import { DhcpAddressEligibility, DhcpAddressEligibilityQuery, DhcpAddressEligibilityResponse, DhcpInfoResponse, DhcpLease, DhcpLeaseCreateRequest, DhcpLeaseDeleteRequest } from '../../../proto/dhcp.pb';
import { IMetaTableEntry } from '../../../types/meta-table-entry.interface';
import { DhcpClientService } from '../../../services/grpc/dhcp-client.service';
import { RemoveDhcpLeaseComponent } from '../../dialogs/remove-dhcp-lease/remove-dhcp-lease.component';
import { SnackService } from '../../../services/snack.service';
import { AddDhcpLeaseComponent } from '../../dialogs/add-dhcp-lease/add-dhcp-lease.component';
import { INewDhcpLease } from '../../../types/new-dhcp-lease-form.interface';
import { DhcpConfirmComponent } from '../../dialogs/dhcp-confirm/dhcp-confirm.component';
import { IProblemData } from '../../../types/problem-data.type';

const PROBLEM_DICTIONARY = {
  [DhcpAddressEligibility.OK]: "",
  [DhcpAddressEligibility.DHCP_OVERLAP]: "Address overlaps with DHCP range",
  [DhcpAddressEligibility.OUT_OF_RANGE]: "Address is not in your network",
  [DhcpAddressEligibility.INFRASTRUCTURE_CONFLICT]: "Address conflicts with network infrastructure",
  [DhcpAddressEligibility.BROADCAST_CONFLICT]: "Cannot use broadcast address",
  [DhcpAddressEligibility.BASE_CONFLICT]: "Cannot use network address",
  [DhcpAddressEligibility.STATIC_LEASE_CONFLICT]: "Address conflicts with another static lease",
  [DhcpAddressEligibility.ACTIVE_LEASE_CONFLICT]: "Address conflicts with another active lease",
};

@Component({
  selector: 'app-dhcp',
  imports: [
    CoreModule,
  ],
  templateUrl: './dhcp.template.html',
  styleUrls: [
    './dhcp.style.scss',
  ]
})
export class DhcpComponent {
  information!: DhcpInfoResponse;
  metaTableEntries!: IMetaTableEntry[];

  private _leases$: BehaviorSubject<DhcpLease[]> = new BehaviorSubject<DhcpLease[]>([]);

  get leases$(): Observable<DhcpLease[]> {
    return this._leases$.asObservable();
  }

  constructor(
    private activatedRoute: ActivatedRoute,
    private dhcpClient: DhcpClientService,
    private dialog: MatDialog,
    private snackService: SnackService,
  ) {
    this.activatedRoute.data.subscribe(data => {
      this.information = data["information"];

      this.metaTableEntries = [
        {
          property: "Network address",
          value: this.information.configuration?.network,
        },
        {
          property: "Network mask",
          value: this.information.configuration?.maskDotted,
        },
        {
          property: "DHCP range",
          value: `${this.information.dhcpRange?.startIp} - ${this.information.dhcpRange?.endIp}`,
        },
        {
          property: "Gateway address",
          value: this.information.routerAddress,
        },
        {
          property: "Broadcast address",
          value: this.information.configuration?.broadcast,
        },
      ];

      this._leases$.next(this.information.leases!);
    });
  }

  createLease(existing?: DhcpLease): void {
    const dialogRef = this.dialog.open(
      AddDhcpLeaseComponent,
      { data: existing, }
    );

    dialogRef.afterClosed().subscribe(res => {
      if (res === null || res === undefined)
        return;

      const newLease = res as INewDhcpLease;
      this.dhcpClient.queryLeaseEligibility(new DhcpAddressEligibilityQuery(newLease))
        .subscribe({
          next: x => this.handleEligibility(newLease, x),
          error: _ => this.snackService.show("Could not validate DHCP lease.", "Dismiss"),
        })
    });
  }

  deleteLease(lease: DhcpLease): void {
    const dialogRef = this.dialog.open(
      RemoveDhcpLeaseComponent,
      { data: lease, }
    );

    dialogRef.afterClosed().subscribe(res => {
      if (!res)
        return;

      this.dhcpClient.deleteLease(new DhcpLeaseDeleteRequest({ id: lease.id, }))
        .subscribe({
          next: _ => this.reloadLeases(),
          error: _ => this.snackService.show("Could not delete DHCP lease.", "Dismiss"),
        })
    });
  }

  private reloadLeases(): void {
    this.dhcpClient.getLeases()
      .subscribe({
        next: y => {
          this.information.leases = y.leases;
          this._leases$.next(this.information.leases!);
        },
        error: _ => this.snackService.show("Could not reload DHCP leases.", "Dismiss"),
      });
  }

  private handleEligibility(lease: INewDhcpLease, result: DhcpAddressEligibilityResponse): void {
    const fails = [];
    const warns = [];
    for (const flag of result.flags) {
      switch (flag) {
        case DhcpAddressEligibility.OK:
          break;

        case DhcpAddressEligibility.DHCP_OVERLAP:
        case DhcpAddressEligibility.STATIC_LEASE_CONFLICT:
        case DhcpAddressEligibility.ACTIVE_LEASE_CONFLICT:
          warns.push(flag);
          break;

        case DhcpAddressEligibility.OUT_OF_RANGE:
        case DhcpAddressEligibility.INFRASTRUCTURE_CONFLICT:
        case DhcpAddressEligibility.BROADCAST_CONFLICT:
        case DhcpAddressEligibility.BASE_CONFLICT:
          fails.push(flag);
          break;
      }
    }

    if (fails.length > 0) {
      const problems = fails.map(x => PROBLEM_DICTIONARY[x]);
      this.dialog.open(
        DhcpConfirmComponent,
        { data: { problems, isFatal: true, } as IProblemData, }
      );
      return;
    }

    if (warns.length > 0) {
      const problems = warns.map(x => PROBLEM_DICTIONARY[x]);
      const dialogRef = this.dialog.open(
        DhcpConfirmComponent,
        { data: { problems, isFatal: false, } as IProblemData, }
      );

      dialogRef.afterClosed().subscribe(x => {
        if (x)
          this.confirmCreateLease(new DhcpLeaseCreateRequest(lease));
      });

      return;
    }

    this.confirmCreateLease(new DhcpLeaseCreateRequest(lease));
  }

  private confirmCreateLease(req: DhcpLeaseCreateRequest): void {
    this.dhcpClient.createLease(req)
      .subscribe({
        next: _ => this.reloadLeases(),
        error: _ => this.snackService.show("Could not create DHCP lease.", "Dismiss"),
      });
  }
}
