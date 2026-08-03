import { Component, inject } from '@angular/core';

import { ToastService } from './toast.service';

const TYPE_CLASSES: Record<string, string> = {
  success: 'bg-green-600',
  error: 'bg-red-600',
  info: 'bg-gray-900'
};

@Component({
  selector: 'app-toast-host',
  standalone: true,
  template: `
    <div class="pointer-events-none fixed inset-x-0 top-4 z-50 flex flex-col items-center gap-2">
      @for (toast of toastService.toasts(); track toast.id) {
        <div
          class="pointer-events-auto flex max-w-sm items-center gap-3 rounded-md px-4 py-3 text-sm text-white shadow-lg"
          [class]="typeClass(toast.type)"
        >
          <span class="flex-1">{{ toast.message }}</span>
          <button
            type="button"
            class="text-white/80 hover:text-white"
            aria-label="Dismiss"
            (click)="toastService.dismiss(toast.id)"
          >
            &times;
          </button>
        </div>
      }
    </div>
  `
})
export class ToastComponent {
  protected readonly toastService = inject(ToastService);

  protected typeClass(type: string): string {
    return TYPE_CLASSES[type] ?? TYPE_CLASSES['info'];
  }
}
