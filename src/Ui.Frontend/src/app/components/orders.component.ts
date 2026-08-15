import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderService } from '../services/order.service';
import { Order } from '../models/order.model';

@Component({
  standalone: true,
  selector: 'app-orders',
  template: `
  <h2>Orders</h2>
  <button (click)="load()">Reload</button>
  <ul>
    <li *ngFor="let o of orders">{{ o.item }} (x{{ o.quantity }}) - {{ o.createdAt || '' }}</li>
  </ul>
  `,
  imports: [CommonModule],
  providers: [OrderService]
})
export class OrdersComponent {
  orders: Order[] = [];

  constructor(private svc: OrderService) {}

  load() {
    this.svc.getAll().subscribe({ next: res => this.orders = res, error: _ => this.orders = [] });
  }

  ngOnInit() {
    this.load();
  }
}
