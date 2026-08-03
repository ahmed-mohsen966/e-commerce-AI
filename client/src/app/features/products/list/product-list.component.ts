import { DecimalPipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { PaginatedList } from '../../../core/models/pagination.model';
import { AuthService } from '../../../core/services/auth.service';
import { extractErrorMessage } from '../../../core/utils/api-error.util';
import { CardComponent } from '../../../shared/ui/card/card.component';
import { PaginationComponent } from '../../../shared/ui/pagination/pagination.component';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { Category } from '../models/category.models';
import { ProductListItem, ProductListQuery, ProductSortBy } from '../models/product.models';
import { CategoryService } from '../services/category.service';
import { ProductService } from '../services/product.service';

const PAGE_SIZE = 12;

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [DecimalPipe, FormsModule, RouterLink, CardComponent, PaginationComponent],
  templateUrl: './product-list.component.html'
})
export class ProductListComponent implements OnInit {
  private readonly productService = inject(ProductService);
  private readonly categoryService = inject(CategoryService);
  private readonly authService = inject(AuthService);
  private readonly toastService = inject(ToastService);

  readonly loading = signal(false);
  readonly result = signal<PaginatedList<ProductListItem> | null>(null);
  readonly categories = signal<Category[]>([]);

  categoryId = '';
  sortBy: ProductSortBy = 'CreatedAt';
  sortDescending = true;
  private pageNumber = 1;

  ngOnInit(): void {
    // GET /categories requires authentication; anonymous visitors just get the
    // unfiltered "All categories" option instead of an error toast on a public page.
    if (this.authService.isAuthenticated()) {
      this.categoryService.list().subscribe({
        next: (categories) => this.categories.set(categories),
        error: (error) => this.toastService.error(extractErrorMessage(error, 'Could not load categories.'))
      });
    }
    this.load();
  }

  onFiltersChange(): void {
    this.pageNumber = 1;
    this.load();
  }

  onPageChange(pageNumber: number): void {
    this.pageNumber = pageNumber;
    this.load();
  }

  private load(): void {
    const query: ProductListQuery = {
      pageNumber: this.pageNumber,
      pageSize: PAGE_SIZE,
      categoryId: this.categoryId || undefined,
      sortBy: this.sortBy,
      sortDescending: this.sortDescending
    };

    this.loading.set(true);
    this.productService
      .list(query)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (result) => this.result.set(result),
        error: (error) => this.toastService.error(extractErrorMessage(error, 'Could not load products.'))
      });
  }
}
