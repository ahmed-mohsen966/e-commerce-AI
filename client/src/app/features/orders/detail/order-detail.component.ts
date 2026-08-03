import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize } from 'rxjs';

import { extractErrorMessage } from '../../../core/utils/api-error.util';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { Order } from '../models/order.models';
import { OrderService } from '../services/order.service';

@Component({
  selector: 'app-order-detail',
  standalone: true,
  imports: [DatePipe, DecimalPipe],
  templateUrl: './order-detail.component.html'
})
export class OrderDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly orderService = inject(OrderService);
  private readonly toastService = inject(ToastService);

  readonly loading = signal(false);
  readonly order = signal<Order | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.router.navigateByUrl('/orders');
      return;
    }

    this.loading.set(true);
    this.orderService
      .getById(id)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (order) => this.order.set(order),
        error: (error) => this.toastService.error(extractErrorMessage(error, 'Order not found.'))
      });
  }
}
