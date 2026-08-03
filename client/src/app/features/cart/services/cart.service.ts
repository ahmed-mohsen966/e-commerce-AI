import { HttpClient } from '@angular/common/http';
import { Injectable, computed, signal } from '@angular/core';
import { Observable, switchMap, tap } from 'rxjs';

import { environment } from '../../../../environments/environment';
import { AddCartItemRequest, Cart, UpdateCartItemRequest } from '../models/cart.models';

const CART_URL = `${environment.apiBaseUrl}/cart`;

@Injectable({ providedIn: 'root' })
export class CartService {
  private readonly cartSignal = signal<Cart | null>(null);

  readonly cart = this.cartSignal.asReadonly();
  readonly itemCount = computed(
    () => this.cartSignal()?.items.reduce((sum, item) => sum + item.quantity, 0) ?? 0
  );

  constructor(private readonly http: HttpClient) {}

  refresh(): Observable<Cart> {
    return this.http.get<Cart>(CART_URL).pipe(tap((cart) => this.cartSignal.set(cart)));
  }

  addItem(request: AddCartItemRequest): Observable<Cart> {
    return this.http
      .post<Cart>(`${CART_URL}/items`, request)
      .pipe(tap((cart) => this.cartSignal.set(cart)));
  }

  updateItem(productVariantId: string, request: UpdateCartItemRequest): Observable<Cart> {
    return this.http
      .put<Cart>(`${CART_URL}/items/${productVariantId}`, request)
      .pipe(tap((cart) => this.cartSignal.set(cart)));
  }

  removeItem(productVariantId: string): Observable<Cart> {
    return this.http
      .delete<void>(`${CART_URL}/items/${productVariantId}`)
      .pipe(switchMap(() => this.refresh()));
  }

  clear(): void {
    this.cartSignal.set(null);
  }
}
