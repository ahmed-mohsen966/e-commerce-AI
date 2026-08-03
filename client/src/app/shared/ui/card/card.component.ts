import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-card',
  standalone: true,
  template: `
    <div class="rounded-lg bg-white p-4 shadow ring-1 ring-black/5">
      @if (title) {
        <h3 class="mb-2 text-base font-semibold text-gray-900">{{ title }}</h3>
      }
      <ng-content></ng-content>
    </div>
  `
})
export class CardComponent {
  @Input() title?: string;
}
