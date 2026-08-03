import { Routes } from '@angular/router';

export const CART_ROUTES: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./page/cart-page.component').then((m) => m.CartPageComponent)
  }
];
