export interface Product {
  id: number;
  name: string;
  category: string;
  description: string;
  price: number;
  stock: number;
  brand: string;
  imageUrl: string;
}

export interface CartItem {
  product: Product;
  quantity: number;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  username: string;
  email: string;
}

export interface CheckoutRequest {
  items: { productId: number; quantity: number }[];
  shippingAddress: string;
  shippingCity: string;
  shippingZip: string;
  phoneNumber: string;
}

export interface OrderResponse {
  orderId: number;
  totalPrice: number;
  status: string;
  createdAt: string;
}