import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Order } from '../models/order.model';

export interface CreateOrderRequest {
  CustomerName: string;
  Amount: number;
}

@Injectable({ providedIn: 'root' })
export class OrderService {
  // Use ApiGateway absolute URL to avoid dev-server proxy configuration issues.
  private base = 'http://localhost:5172/api/v1/orders';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Order[]> {
    return this.http.get<Order[]>(this.base);
  }

  create(payload: CreateOrderRequest): Observable<Order> {
    return this.http.post<Order>(this.base, payload);
  }
}
