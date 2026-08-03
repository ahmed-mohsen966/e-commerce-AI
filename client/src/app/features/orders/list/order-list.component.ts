import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { extractErrorMessage } from '../../../core/utils/api-error.util';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { OrderSummary } from '../models/order.models';
import { OrderService } from '../services/order.service';

@Component({
  selector: 'app-order-list',
  standalone: true,
  imports: [DatePipe, DecimalPipe, RouterLink],
  templateUrl: './order-list.component.html'
})
export class OrderListComponent implements OnInit {
  private readonly orderService = inject(OrderService);
  private readonly toastService = inject(ToastService);

  readonly loading = signal(false);
  readonly orders = signal<OrderSummary[]>([]);

  ngOnInit(): void {
    this.loading.set(true);
    this.orderService
      .getMine()
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (orders) => this.orders.set(orders),
        error: (error) => this.toastService.error(extractErrorMessage(error, 'Could not load your orders.'))
      });
  }
}
