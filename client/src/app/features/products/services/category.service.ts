import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';
import { Category, CreateCategoryRequest } from '../models/category.models';

const CATEGORIES_URL = `${environment.apiBaseUrl}/categories`;

@Injectable({ providedIn: 'root' })
export class CategoryService {
  constructor(private readonly http: HttpClient) {}

  list(): Observable<Category[]> {
    return this.http.get<Category[]>(CATEGORIES_URL);
  }

  create(request: CreateCategoryRequest): Observable<string> {
    return this.http.post<string>(CATEGORIES_URL, request);
  }
}
