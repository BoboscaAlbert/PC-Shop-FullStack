import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-list.html'
})
export class ProductListComponent implements OnInit {
  products: any[] = [];
  loading: boolean = true;

  constructor(
    private http: HttpClient,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadProducts();
  }

  loadProducts() {
    this.loading = true;
    this.http.get<any[]>('https://localhost:7101/api/Products').subscribe({
      next: (data) => {
        this.products = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  onAddToCart(product: any) {
    const cartPayload = {
      productId: product.id || product.Id,
      name: product.name || product.Name,
      price: product.price || product.Price,
      imageUrl: product.imageUrl || product.ImageUrl,
      quantity: 1
    };

    this.http.post('https://localhost:7101/api/Cart', cartPayload).subscribe({
      next: () => {
        alert(`Succes! ${product.name || product.Name} este în coș.`);
      },
      error: (err) => {
        console.error(err);
        alert('Eroare la adăugarea în coș.');
      }
    });
  }
}