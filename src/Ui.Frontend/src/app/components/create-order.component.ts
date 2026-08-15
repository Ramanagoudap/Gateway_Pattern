import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OrderService } from '../services/order.service';
import { Order } from '../models/order.model';

@Component({
  standalone: true,
  selector: 'app-create-order',
  template: `
  <h2>Create Order</h2>
  <form (ngSubmit)="submit()">
    <div>
      <label>Item: <input [(ngModel)]="model.item" name="item" required></label>
    </div>
    <div>
      <label>Quantity: <input type="number" [(ngModel)]="model.quantity" name="quantity" required></label>
    </div>
    <button type="submit">Create</button>
  </form>
  <pre *ngIf="result">{{ result | json }}</pre>
  `,
  imports: [CommonModule, FormsModule],
  providers: [OrderService]
})
export class CreateOrderComponent {
  model: Order = { item: '', quantity: 1 };
  result: any = null;

  constructor(private svc: OrderService) {}

  submit() {
    this.svc.create(this.model).subscribe({ next: r => this.result = r, error: e => this.result = { error: e?.message } });
  }
}
