import { Routes } from '@angular/router';

export const PRODUCTS_ROUTES: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./list/product-list.component').then((m) => m.ProductListComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./detail/product-detail.component').then((m) => m.ProductDetailComponent)
  }
];
