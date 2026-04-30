import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product } from '../models'; // Verifică dacă models.ts e în src/app

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  // Folosim portul tău 7101 pe care l-am văzut în Swagger
  private apiUrl = 'https://localhost:7101/api/Products';

  constructor(private http: HttpClient) { }

  getProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(this.apiUrl);
  }
}