import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  imports: [CommonModule]
})
export class AppComponent {
  title = 'Ui.Frontend';
  ordersResult: any = null;
  notificationsResult: any = null;

  constructor(private http: HttpClient) {}

  getOrders() {
    this.http.get('/api/v1/orders').subscribe({
      next: res => this.ordersResult = res,
      error: err => this.ordersResult = { error: err.message }
    });
  }

  createOrder() {
    const payload = { item: 'Sample item', quantity: 1 };
    this.http.post('/api/v1/orders', payload).subscribe({
      next: res => this.ordersResult = res,
      error: err => this.ordersResult = { error: err.message }
    });
  }

  sendNotification() {
    const payload = { to: 'user@example.com', message: 'Test notification' };
    this.http.post('/api/v1/notifications', payload).subscribe({
      next: res => this.notificationsResult = res,
      error: err => this.notificationsResult = { error: err.message }
    });
  }
}
