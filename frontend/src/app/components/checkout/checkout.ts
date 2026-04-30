import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './checkout.html'
})
export class CheckoutComponent implements OnInit {
  cartItems: any[] = [];
  totalToPay: number = 0; // Redenumit pentru a se potrivi cu eroarea din HTML
  
  shippingDetails = { // Redenumit din shippingData pentru a se potrivi cu HTML-ul
    fullName: '',
    address: '',
    phone: ''
  };

  constructor(
    private http: HttpClient, 
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadCartSummary();
  }

  loadCartSummary() {
    this.http.get<any[]>('https://localhost:7101/api/Cart').subscribe({
      next: (data) => {
        this.cartItems = data;
        this.calculateTotal();
        this.cdr.detectChanges();
      },
      error: (err) => console.error(err)
    });
  }

  calculateTotal() {
    this.totalToPay = this.cartItems.reduce((acc, item) => {
      const p = item.price || item.Price || 0;
      const q = item.quantity || item.Quantity || 0;
      return acc + (p * q);
    }, 0);
  }

  onPlaceOrder() { // Redenumit din confirmOrder pentru a repara eroarea din HTML
    if (!this.shippingDetails.address || !this.shippingDetails.phone) {
      alert('Completează datele!');
      return;
    }

    const payload = {
      customerName: this.shippingDetails.fullName,
      shippingAddress: this.shippingDetails.address,
      phone: this.shippingDetails.phone,
      totalAmount: this.totalToPay,
      items: this.cartItems
    };

    this.http.post('https://localhost:7101/api/Orders/checkout', payload).subscribe({
      next: () => {
        this.http.delete('https://localhost:7101/api/Cart').subscribe(() => {
          alert('Comandă reușită!');
          this.router.navigate(['/products']);
        });
      },
      error: (err) => alert('Eroare la trimiterea comenzii.')
    });
  }
}