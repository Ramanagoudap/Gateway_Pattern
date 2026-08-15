import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { NotificationPayload } from '../models/notification.model';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  // Use ApiGateway absolute URL to avoid dev-server proxy configuration issues.
  private base = 'http://localhost:5172/api/v1/notifications';

  constructor(private http: HttpClient) {}

  send(payload: NotificationPayload): Observable<any> {
    return this.http.post(this.base, payload);
  }
}
