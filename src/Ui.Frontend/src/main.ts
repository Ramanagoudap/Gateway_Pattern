import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { AppComponent } from './app/app.component';
import { OrdersComponent } from './app/components/orders.component';
import { CreateOrderComponent } from './app/components/create-order.component';
import { NotificationsComponent } from './app/components/notifications.component';

const routes = [
  { path: '', component: OrdersComponent },
  { path: 'create-order', component: CreateOrderComponent },
  { path: 'notifications', component: NotificationsComponent }
];

bootstrapApplication(AppComponent, {
  providers: [
    provideHttpClient(),
    provideRouter(routes)
  ]
}).catch(err => console.error(err));
