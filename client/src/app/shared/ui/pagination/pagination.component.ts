import { Component, EventEmitter, Input, Output } from '@angular/core';

import { ButtonComponent } from '../button/button.component';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [ButtonComponent],
  template: `
    <div class="flex items-center justify-between gap-4">
      <p class="text-sm text-gray-600">Page {{ pageNumber }} of {{ totalPages || 1 }}</p>
      <div class="flex gap-2">
        <app-button variant="secondary" size="sm" [disabled]="!hasPreviousPage" (click)="pageChange.emit(pageNumber - 1)">
          Previous
        </app-button>
        <app-button variant="secondary" size="sm" [disabled]="!hasNextPage" (click)="pageChange.emit(pageNumber + 1)">
          Next
        </app-button>
      </div>
    </div>
  `
})
export class PaginationComponent {
  @Input() pageNumber = 1;
  @Input() totalPages = 1;
  @Input() hasPreviousPage = false;
  @Input() hasNextPage = false;

  @Output() pageChange = new EventEmitter<number>();
}
