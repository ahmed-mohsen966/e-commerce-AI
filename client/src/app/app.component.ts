import { Component, OnInit, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { catchError, of } from 'rxjs';

import { AuthService } from './core/services/auth.service';
import { CartService } from './features/cart/services/cart.service';
import { ToastComponent } from './shared/ui/toast/toast.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, ToastComponent],
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  protected readonly authService = inject(AuthService);
  protected readonly cartService = inject(CartService);
  private readonly router = inject(Router);

  ngOnInit(): void {
    if (this.authService.isAuthenticated()) {
      this.cartService
        .refresh()
        .pipe(catchError(() => of(null)))
        .subscribe();
    }
  }

  logout(): void {
    this.authService.logout();
    this.cartService.clear();
    this.router.navigate(['/auth/login']);
  }
}
