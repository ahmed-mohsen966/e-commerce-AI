import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';

import { PaginatedList } from '../../../core/models/pagination.model';
import { extractErrorMessage } from '../../../core/utils/api-error.util';
import { ButtonComponent } from '../../../shared/ui/button/button.component';
import { PaginationComponent } from '../../../shared/ui/pagination/pagination.component';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { OrderStatus, OrderSummary } from '../../orders/models/order.models';
import { OrderService } from '../../orders/services/order.service';

const PAGE_SIZE = 20;
const ORDER_STATUSES: OrderStatus[] = ['Pending', 'Paid', 'Shipped', 'Delivered', 'Cancelled'];

@Component({
  selector: 'app-admin-orders',
  standalone: true,
  imports: [DatePipe, DecimalPipe, FormsModule, ButtonComponent, PaginationComponent],
  templateUrl: './admin-orders.component.html'
})
export class AdminOrdersComponent implements OnInit {
  private readonly orderService = inject(OrderService);
  private readonly toastService = inject(ToastService);

  readonly statuses = ORDER_STATUSES;
  readonly loading = signal(false);
  readonly result = signal<PaginatedList<OrderSummary> | null>(null);
  readonly savingOrderId = signal<string | null>(null);

  statusFilter: OrderStatus | '' = '';
  private pageNumber = 1;

  readonly pendingStatus = new Map<string, OrderStatus>();
  readonly pendingReason = new Map<string, string>();

  ngOnInit(): void {
    this.load();
  }

  onFilterChange(): void {
    this.pageNumber = 1;
    this.load();
  }

  onPageChange(pageNumber: number): void {
    this.pageNumber = pageNumber;
    this.load();
  }

  statusFor(order: OrderSummary): OrderStatus {
    return this.pendingStatus.get(order.id) ?? order.status;
  }

  reasonFor(order: OrderSummary): string {
    return this.pendingReason.get(order.id) ?? '';
  }

  onStatusChange(order: OrderSummary, status: OrderStatus): void {
    this.pendingStatus.set(order.id, status);
  }

  onReasonChange(order: OrderSummary, reason: string): void {
    this.pendingReason.set(order.id, reason);
  }

  saveStatus(order: OrderSummary): void {
    const newStatus = this.statusFor(order);
    const cancellationReason = this.reasonFor(order);

    if (newStatus === 'Cancelled' && !cancellationReason.trim()) {
      this.toastService.error('A cancellation reason is required.');
      return;
    }

    this.savingOrderId.set(order.id);
    this.orderService
      .updateStatus(order.id, {
        orderId: order.id,
        newStatus,
        cancellationReason: newStatus === 'Cancelled' ? cancellationReason : null
      })
      .pipe(finalize(() => this.savingOrderId.set(null)))
      .subscribe({
        next: () => {
          this.toastService.success('Order status updated.');
          this.load();
        },
        error: (error) => this.toastService.error(extractErrorMessage(error, 'Could not update order status.'))
      });
  }

  private load(): void {
    this.loading.set(true);
    this.orderService
      .getAll({ pageNumber: this.pageNumber, pageSize: PAGE_SIZE, status: this.statusFilter || undefined })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (result) => this.result.set(result),
        error: (error) => this.toastService.error(extractErrorMessage(error, 'Could not load orders.'))
      });
  }
}
