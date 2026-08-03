import { Routes } from '@angular/router';

export const ORDERS_ROUTES: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./list/order-list.component').then((m) => m.OrderListComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./detail/order-detail.component').then((m) => m.OrderDetailComponent)
  }
];
