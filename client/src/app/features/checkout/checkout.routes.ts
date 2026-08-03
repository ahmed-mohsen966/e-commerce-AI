import { Routes } from '@angular/router';

export const CHECKOUT_ROUTES: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./page/checkout-page.component').then((m) => m.CheckoutPageComponent)
  }
];
