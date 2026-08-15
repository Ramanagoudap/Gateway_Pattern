import { Routes } from '@angular/router';
import { OrdersComponent } from './components/orders.component';
import { CreateOrderComponent } from './components/create-order.component';
import { NotificationsComponent } from './components/notifications.component';

export const routes: Routes = [
  { path: '', component: OrdersComponent },
  { path: 'create-order', component: CreateOrderComponent },
  { path: 'notifications', component: NotificationsComponent }
];
