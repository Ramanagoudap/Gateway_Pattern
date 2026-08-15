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
      <label>Customer Name: <input [(ngModel)]="customerName" name="customerName" required></label>
    </div>
    <div>
      <label>Amount: <input type="number" [(ngModel)]="amount" name="amount" required></label>
    </div>
    <button type="submit">Create</button>
  </form>
  <pre *ngIf="result">{{ result | json }}</pre>
  `,
  imports: [CommonModule, FormsModule],
  providers: [OrderService]
})
export class CreateOrderComponent {
  customerName = '';
  amount = 1;
  result: any = null;

  constructor(private svc: OrderService) {}

  submit() {
    const payload = { CustomerName: this.customerName, Amount: this.amount };
    this.svc.create(payload).subscribe({ next: r => this.result = r, error: e => this.result = { error: e?.message } });
  }
}
