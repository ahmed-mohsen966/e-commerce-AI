import { DecimalPipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize, of, switchMap } from 'rxjs';

import { extractErrorMessage } from '../../../core/utils/api-error.util';
import { ButtonComponent } from '../../../shared/ui/button/button.component';
import { CardComponent } from '../../../shared/ui/card/card.component';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { CartService } from '../../cart/services/cart.service';
import { OrderService } from '../../orders/services/order.service';
import { AddressService } from '../services/address.service';

@Component({
  selector: 'app-checkout-page',
  standalone: true,
  imports: [DecimalPipe, ReactiveFormsModule, ButtonComponent, CardComponent],
  templateUrl: './checkout-page.component.html'
})
export class CheckoutPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly addressService = inject(AddressService);
  private readonly orderService = inject(OrderService);
  protected readonly cartService = inject(CartService);
  private readonly toastService = inject(ToastService);
  private readonly router = inject(Router);

  readonly placingOrder = signal(false);
  readonly sameAsShipping = signal(true);

  readonly shippingForm = this.fb.nonNullable.group({
    line1: ['', Validators.required],
    line2: [''],
    city: ['', Validators.required],
    state: ['', Validators.required],
    postalCode: ['', Validators.required],
    country: ['', Validators.required]
  });

  readonly billingForm = this.fb.nonNullable.group({
    line1: ['', Validators.required],
    line2: [''],
    city: ['', Validators.required],
    state: ['', Validators.required],
    postalCode: ['', Validators.required],
    country: ['', Validators.required]
  });

  ngOnInit(): void {
    if (!this.cartService.cart()) {
      this.cartService.refresh().subscribe();
    }
  }

  toggleSameAsShipping(value: boolean): void {
    this.sameAsShipping.set(value);
  }

  placeOrder(): void {
    this.shippingForm.markAllAsTouched();
    if (this.shippingForm.invalid) {
      return;
    }
    if (!this.sameAsShipping()) {
      this.billingForm.markAllAsTouched();
      if (this.billingForm.invalid) {
        return;
      }
    }

    this.placingOrder.set(true);

    this.addressService
      .create({ ...this.shippingForm.getRawValue(), type: 'Shipping', isDefault: true })
      .pipe(
        switchMap((shippingAddress) => {
          if (this.sameAsShipping()) {
            return of({ shippingAddressId: shippingAddress.id, billingAddressId: shippingAddress.id });
          }
          return this.addressService.create({ ...this.billingForm.getRawValue(), type: 'Billing' }).pipe(
            switchMap((billingAddress) =>
              of({ shippingAddressId: shippingAddress.id, billingAddressId: billingAddress.id })
            )
          );
        }),
        switchMap((addressIds) => this.orderService.place(addressIds)),
        finalize(() => this.placingOrder.set(false))
      )
      .subscribe({
        next: (orderId) => {
          this.cartService.clear();
          this.toastService.success('Order placed!');
          this.router.navigate(['/orders', orderId]);
        },
        error: (error) => this.toastService.error(extractErrorMessage(error, 'Could not place your order.'))
      });
  }
}
