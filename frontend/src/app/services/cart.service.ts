import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

// Definim cum arată un produs în coș (produsul + cantitatea)
export interface CartItem {
  product: any; 
  quantity: number;
}

@Injectable({
  providedIn: 'root'
})
export class CartService {
 
  private cartItems = new BehaviorSubject<CartItem[]>([]);
  
 
  cartItems$ = this.cartItems.asObservable();

  constructor() {}

  addToCart(product: any) {
    const currentItems = this.cartItems.value;
    const existingItem = currentItems.find(item => item.product.id === product.id);

    if (existingItem) {
    
      existingItem.quantity += 1;
      this.cartItems.next([...currentItems]);
    } else {
      
      this.cartItems.next([...currentItems, { product, quantity: 1 }]);
    }
    
    
    console.log('Coșul curent:', this.cartItems.value);
  }

  
  clearCart() {
    this.cartItems.next([]);
  }
}