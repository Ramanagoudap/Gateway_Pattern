import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-nav',
  template: `
  <nav style="padding:8px; background:#eee;">
    <a routerLink="/" style="margin-right:8px;">Orders</a>
    <a routerLink="/create-order" style="margin-right:8px;">Create Order</a>
    <a routerLink="/notifications">Notifications</a>
  </nav>
  `,
  imports: [RouterModule]
})
export class NavComponent {}
