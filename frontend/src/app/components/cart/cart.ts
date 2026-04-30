import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './cart.html'
})
export class CartComponent implements OnInit {
  items: any[] = [];

  constructor(private http: HttpClient, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.loadCart();
  }

  loadCart() {
    this.http.get<any[]>('https://localhost:7101/api/Cart').subscribe({
      next: (data) => {
        this.items = data;
        this.cdr.detectChanges();
      }
    });
  }

  removeItem(index: number) {
    this.http.delete(`https://localhost:7101/api/Cart/${index}`).subscribe({
      next: () => {
        this.loadCart();
      }
    });
  }

  getTotal() {
    return this.items.reduce((acc, item) => {
      const p = item.price || item.Price || 0;
      const q = item.quantity || item.Quantity || 0;
      return acc + (p * q);
    }, 0);
  }
}