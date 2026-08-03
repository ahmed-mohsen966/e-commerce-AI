import { DecimalPipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize } from 'rxjs';

import { extractErrorMessage } from '../../../core/utils/api-error.util';
import { AuthService } from '../../../core/services/auth.service';
import { ButtonComponent } from '../../../shared/ui/button/button.component';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { CartService } from '../../cart/services/cart.service';
import { Product, ProductVariant } from '../models/product.models';
import { ProductService } from '../services/product.service';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [DecimalPipe, FormsModule, ButtonComponent],
  templateUrl: './product-detail.component.html'
})
export class ProductDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly productService = inject(ProductService);
  private readonly cartService = inject(CartService);
  private readonly authService = inject(AuthService);
  private readonly toastService = inject(ToastService);

  readonly loading = signal(false);
  readonly product = signal<Product | null>(null);
  readonly addingToCart = signal(false);

  selectedVariantId = '';
  quantity = 1;

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.router.navigateByUrl('/products');
      return;
    }

    this.loading.set(true);
    this.productService
      .getById(id)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (product) => {
          this.product.set(product);
          this.selectedVariantId = product.variants[0]?.id ?? '';
        },
        error: (error) => this.toastService.error(extractErrorMessage(error, 'Product not found.'))
      });
  }

  get selectedVariant(): ProductVariant | undefined {
    return this.product()?.variants.find((v) => v.id === this.selectedVariantId);
  }

  addToCart(): void {
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/auth/login'], { queryParams: { returnUrl: this.router.url } });
      return;
    }
    if (!this.selectedVariantId || this.quantity < 1) {
      return;
    }

    this.addingToCart.set(true);
    this.cartService
      .addItem({ productVariantId: this.selectedVariantId, quantity: this.quantity })
      .pipe(finalize(() => this.addingToCart.set(false)))
      .subscribe({
        next: () => this.toastService.success('Added to cart.'),
        error: (error) => this.toastService.error(extractErrorMessage(error, 'Could not add to cart.'))
      });
  }
}
