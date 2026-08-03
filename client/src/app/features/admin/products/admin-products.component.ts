import { DecimalPipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs';

import { extractErrorMessage } from '../../../core/utils/api-error.util';
import { ButtonComponent } from '../../../shared/ui/button/button.component';
import { CardComponent } from '../../../shared/ui/card/card.component';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { Category } from '../../products/models/category.models';
import { Product, ProductListItem } from '../../products/models/product.models';
import { CategoryService } from '../../products/services/category.service';
import { ProductService } from '../../products/services/product.service';

@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [DecimalPipe, ReactiveFormsModule, ButtonComponent, CardComponent],
  templateUrl: './admin-products.component.html'
})
export class AdminProductsComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly productService = inject(ProductService);
  private readonly categoryService = inject(CategoryService);
  private readonly toastService = inject(ToastService);

  readonly loading = signal(false);
  readonly products = signal<ProductListItem[]>([]);
  readonly categories = signal<Category[]>([]);

  readonly creating = signal(false);
  readonly editingProductId = signal<string | null>(null);
  readonly savingEdit = signal(false);
  readonly variantFormProductId = signal<string | null>(null);
  readonly savingVariant = signal(false);

  readonly createForm = this.fb.nonNullable.group({
    name: ['', Validators.required],
    description: ['', Validators.required],
    categoryId: ['', Validators.required],
    basePriceAmount: [0, [Validators.required, Validators.min(0)]],
    basePriceCurrency: ['USD', [Validators.required, Validators.pattern(/^[A-Z]{3}$/)]]
  });

  readonly editForm = this.fb.nonNullable.group({
    name: ['', Validators.required],
    description: ['', Validators.required],
    categoryId: ['', Validators.required],
    basePriceAmount: [0, [Validators.required, Validators.min(0)]],
    basePriceCurrency: ['USD', [Validators.required, Validators.pattern(/^[A-Z]{3}$/)]],
    isActive: [true]
  });

  readonly variantForm = this.fb.nonNullable.group({
    sku: ['', Validators.required],
    size: ['', Validators.required],
    color: ['', Validators.required],
    priceAmount: [0, [Validators.required, Validators.min(0)]],
    priceCurrency: ['USD', [Validators.required, Validators.pattern(/^[A-Z]{3}$/)]],
    stockQuantity: [0, [Validators.required, Validators.min(0)]]
  });

  ngOnInit(): void {
    this.loadCategories();
    this.loadProducts();
  }

  private loadCategories(): void {
    this.categoryService.list().subscribe({
      next: (categories) => this.categories.set(categories),
      error: (error) => this.toastService.error(extractErrorMessage(error, 'Could not load categories.'))
    });
  }

  private loadProducts(): void {
    this.loading.set(true);
    this.productService
      .list({ pageNumber: 1, pageSize: 100 })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (result) => this.products.set(result.items),
        error: (error) => this.toastService.error(extractErrorMessage(error, 'Could not load products.'))
      });
  }

  categoryName(categoryId: string): string {
    return this.categories().find((c) => c.id === categoryId)?.name ?? categoryId;
  }

  createProduct(): void {
    if (this.createForm.invalid) {
      this.createForm.markAllAsTouched();
      return;
    }

    this.creating.set(true);
    this.productService
      .create(this.createForm.getRawValue())
      .pipe(finalize(() => this.creating.set(false)))
      .subscribe({
        next: () => {
          this.toastService.success('Product created.');
          this.createForm.reset({ basePriceAmount: 0, basePriceCurrency: 'USD' });
          this.loadProducts();
        },
        error: (error) => this.toastService.error(extractErrorMessage(error, 'Could not create product.'))
      });
  }

  startEdit(product: ProductListItem): void {
    this.variantFormProductId.set(null);
    this.productService.getById(product.id).subscribe({
      next: (full: Product) => {
        this.editForm.reset({
          name: full.name,
          description: full.description,
          categoryId: full.categoryId,
          basePriceAmount: full.basePriceAmount,
          basePriceCurrency: full.basePriceCurrency,
          isActive: full.isActive
        });
        this.editingProductId.set(product.id);
      },
      error: (error) => this.toastService.error(extractErrorMessage(error, 'Could not load product.'))
    });
  }

  cancelEdit(): void {
    this.editingProductId.set(null);
  }

  saveEdit(): void {
    const id = this.editingProductId();
    if (!id || this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      return;
    }

    this.savingEdit.set(true);
    this.productService
      .update({ id, ...this.editForm.getRawValue() })
      .pipe(finalize(() => this.savingEdit.set(false)))
      .subscribe({
        next: () => {
          this.toastService.success('Product updated.');
          this.editingProductId.set(null);
          this.loadProducts();
        },
        error: (error) => this.toastService.error(extractErrorMessage(error, 'Could not update product.'))
      });
  }

  deleteProduct(product: ProductListItem): void {
    if (!confirm(`Delete "${product.name}"? This cannot be undone.`)) {
      return;
    }
    this.productService.delete(product.id).subscribe({
      next: () => {
        this.toastService.success('Product deleted.');
        this.loadProducts();
      },
      error: (error) => this.toastService.error(extractErrorMessage(error, 'Could not delete product.'))
    });
  }

  startAddVariant(product: ProductListItem): void {
    this.editingProductId.set(null);
    this.variantForm.reset({ priceAmount: 0, priceCurrency: 'USD', stockQuantity: 0 });
    this.variantFormProductId.set(product.id);
  }

  cancelAddVariant(): void {
    this.variantFormProductId.set(null);
  }

  saveVariant(): void {
    const productId = this.variantFormProductId();
    if (!productId || this.variantForm.invalid) {
      this.variantForm.markAllAsTouched();
      return;
    }

    this.savingVariant.set(true);
    this.productService
      .addVariant(productId, this.variantForm.getRawValue())
      .pipe(finalize(() => this.savingVariant.set(false)))
      .subscribe({
        next: () => {
          this.toastService.success('Variant added.');
          this.variantFormProductId.set(null);
        },
        error: (error) => this.toastService.error(extractErrorMessage(error, 'Could not add variant.'))
      });
  }
}
