import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';
import { PaginatedList } from '../../../core/models/pagination.model';
import {
  CreateProductRequest,
  CreateProductVariantRequest,
  Product,
  ProductListItem,
  ProductListQuery,
  UpdateProductRequest
} from '../models/product.models';

const PRODUCTS_URL = `${environment.apiBaseUrl}/products`;

@Injectable({ providedIn: 'root' })
export class ProductService {
  constructor(private readonly http: HttpClient) {}

  list(query: ProductListQuery): Observable<PaginatedList<ProductListItem>> {
    let params = new HttpParams();
    for (const [key, value] of Object.entries(query)) {
      if (value !== undefined && value !== null && value !== '') {
        params = params.set(key, String(value));
      }
    }
    return this.http.get<PaginatedList<ProductListItem>>(PRODUCTS_URL, { params });
  }

  getById(id: string): Observable<Product> {
    return this.http.get<Product>(`${PRODUCTS_URL}/${id}`);
  }

  create(request: CreateProductRequest): Observable<string> {
    return this.http.post<string>(PRODUCTS_URL, request);
  }

  update(request: UpdateProductRequest): Observable<void> {
    return this.http.put<void>(`${PRODUCTS_URL}/${request.id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${PRODUCTS_URL}/${id}`);
  }

  addVariant(productId: string, request: CreateProductVariantRequest): Observable<string> {
    return this.http.post<string>(`${PRODUCTS_URL}/${productId}/variants`, request);
  }
}
