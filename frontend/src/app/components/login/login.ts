import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html'
})
export class LoginComponent {
  credentials = {
    email: '',
    password: ''
  };

  constructor(private authService: AuthService) {}

  onLogin() {
    this.authService.login(this.credentials).subscribe({
      next: () => {
        window.location.href = '/products';
      },
      error: () => {
        alert('Eroare la autentificare. Verifică email-ul și parola.');
      }
    });
  }
}