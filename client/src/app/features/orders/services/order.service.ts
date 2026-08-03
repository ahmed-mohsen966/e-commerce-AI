import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';
import { PaginatedList } from '../../../core/models/pagination.model';
import {
  GetAllOrdersQuery,
  Order,
  OrderSummary,
  PlaceOrderRequest,
  UpdateOrderStatusRequest
} from '../models/order.models';

const ORDERS_URL = `${environment.apiBaseUrl}/orders`;

@Injectable({ providedIn: 'root' })
export class OrderService {
  constructor(private readonly http: HttpClient) {}

  place(request: PlaceOrderRequest): Observable<string> {
    return this.http.post<string>(ORDERS_URL, request);
  }

  getMine(): Observable<OrderSummary[]> {
    return this.http.get<OrderSummary[]>(ORDERS_URL);
  }

  getById(id: string): Observable<Order> {
    return this.http.get<Order>(`${ORDERS_URL}/${id}`);
  }

  getAll(query: GetAllOrdersQuery): Observable<PaginatedList<OrderSummary>> {
    let params = new HttpParams();
    for (const [key, value] of Object.entries(query)) {
      if (value !== undefined && value !== null && value !== '') {
        params = params.set(key, String(value));
      }
    }
    return this.http.get<PaginatedList<OrderSummary>>(`${ORDERS_URL}/all`, { params });
  }

  updateStatus(id: string, request: UpdateOrderStatusRequest): Observable<void> {
    return this.http.put<void>(`${ORDERS_URL}/${id}/status`, request);
  }
}
