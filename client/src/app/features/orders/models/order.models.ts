export type OrderStatus = 'Pending' | 'Paid' | 'Shipped' | 'Delivered' | 'Cancelled';

export interface OrderSummary {
  id: string;
  customerId: string;
  status: OrderStatus;
  orderDate: string;
  totalAmount: number;
  totalCurrency: string;
  itemCount: number;
}

export interface OrderItem {
  productVariantId: string;
  productName: string;
  sku: string;
  quantity: number;
  unitPriceAmount: number;
  unitPriceCurrency: string;
  lineTotalAmount: number;
  lineTotalCurrency: string;
}

export interface Order {
  id: string;
  customerId: string;
  shippingAddressId: string;
  billingAddressId: string;
  status: OrderStatus;
  orderDate: string;
  cancellationReason: string | null;
  totalAmount: number;
  totalCurrency: string;
  items: OrderItem[];
}

export interface PlaceOrderRequest {
  shippingAddressId: string;
  billingAddressId: string;
}

export interface UpdateOrderStatusRequest {
  orderId: string;
  newStatus: OrderStatus;
  cancellationReason?: string | null;
}

export interface GetAllOrdersQuery {
  pageNumber?: number;
  pageSize?: number;
  status?: OrderStatus;
}
