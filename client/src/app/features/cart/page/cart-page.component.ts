import { DecimalPipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { extractErrorMessage } from '../../../core/utils/api-error.util';
import { ButtonComponent } from '../../../shared/ui/button/button.component';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { CartService } from '../services/cart.service';

@Component({
  selector: 'app-cart-page',
  standalone: true,
  imports: [DecimalPipe, FormsModule, RouterLink, ButtonComponent],
  templateUrl: './cart-page.component.html'
})
export class CartPageComponent implements OnInit {
  protected readonly cartService = inject(CartService);
  private readonly toastService = inject(ToastService);

  readonly loading = signal(false);
  readonly pendingVariantId = signal<string | null>(null);

  ngOnInit(): void {
    this.loading.set(true);
    this.cartService
      .refresh()
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        error: (error) => this.toastService.error(extractErrorMessage(error, 'Could not load your cart.'))
      });
  }

  updateQuantity(productVariantId: string, quantity: number): void {
    if (quantity < 1) {
      return;
    }
    this.pendingVariantId.set(productVariantId);
    this.cartService
      .updateItem(productVariantId, { quantity })
      .pipe(finalize(() => this.pendingVariantId.set(null)))
      .subscribe({
        error: (error) => this.toastService.error(extractErrorMessage(error, 'Could not update quantity.'))
      });
  }

  removeItem(productVariantId: string): void {
    this.pendingVariantId.set(productVariantId);
    this.cartService
      .removeItem(productVariantId)
      .pipe(finalize(() => this.pendingVariantId.set(null)))
      .subscribe({
        error: (error) => this.toastService.error(extractErrorMessage(error, 'Could not remove item.'))
      });
  }
}
