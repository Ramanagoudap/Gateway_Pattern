export interface Order {
  id?: string;
  customerName: string;
  amount: number;
  createdAt?: string;
  notificationSent?: boolean;
}
