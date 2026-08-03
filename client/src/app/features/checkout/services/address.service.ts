import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';
import { AddAddressRequest, Address } from '../models/address.models';

const ADDRESSES_URL = `${environment.apiBaseUrl}/addresses`;

/**
 * The backend only exposes POST /addresses (create) — there is no list/get-by-id
 * endpoint yet, so previously-saved addresses can't be fetched or reused across
 * sessions. Checkout collects a fresh address on every order as a result.
 */
@Injectable({ providedIn: 'root' })
export class AddressService {
  constructor(private readonly http: HttpClient) {}

  create(request: AddAddressRequest): Observable<Address> {
    return this.http.post<Address>(ADDRESSES_URL, request);
  }
}
