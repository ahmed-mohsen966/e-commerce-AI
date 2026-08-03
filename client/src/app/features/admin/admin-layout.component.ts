import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  template: `
    <div class="flex flex-col gap-6">
      <nav class="flex gap-4 border-b border-gray-200 pb-2">
        <a
          routerLink="products"
          routerLinkActive="border-indigo-600 text-indigo-600"
          class="border-b-2 border-transparent pb-2 text-sm font-medium text-gray-600 hover:text-indigo-600"
        >
          Products
        </a>
        <a
          routerLink="orders"
          routerLinkActive="border-indigo-600 text-indigo-600"
          class="border-b-2 border-transparent pb-2 text-sm font-medium text-gray-600 hover:text-indigo-600"
        >
          Orders
        </a>
      </nav>
      <router-outlet></router-outlet>
    </div>
  `
})
export class AdminLayoutComponent {}
