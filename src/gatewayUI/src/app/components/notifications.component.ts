import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NotificationService } from '../services/notification.service';

@Component({
  standalone: true,
  selector: 'app-notifications',
  template: `
  <h2>Send Notification</h2>
  <form (ngSubmit)="send()">
    <div>
      <label>To: <input [(ngModel)]="to" name="to" required></label>
    </div>
    <div>
      <label>Message: <input [(ngModel)]="message" name="message" required></label>
    </div>
    <button type="submit">Send</button>
  </form>
  <pre *ngIf="result">{{ result | json }}</pre>
  `,
  imports: [CommonModule, FormsModule],
  providers: [NotificationService]
})
export class NotificationsComponent {
  to = '';
  message = '';
  result: any = null;

  constructor(private svc: NotificationService) {}

  send() {
    this.svc.send({ to: this.to, message: this.message }).subscribe({ next: r => this.result = r, error: e => this.result = { error: e?.message } });
  }
}
