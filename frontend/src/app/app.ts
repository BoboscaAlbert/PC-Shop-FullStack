import { Component, ChangeDetectorRef } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class AppComponent {
  constructor(private router: Router, private cdr: ChangeDetectorRef) {}

  get isLoggedIn(): boolean {
    if (typeof window !== 'undefined') {
      const hasToken = !!localStorage.getItem('token');
      return hasToken;
    }
    return false;
  }

  onLogout() {
    localStorage.removeItem('token');
    this.cdr.detectChanges();
    this.router.navigate(['/login']);
  }
}