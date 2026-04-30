import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.html'
})
export class RegisterComponent {
  userData = {
    FullName: '',
    Email: '',
    Password: ''
  };

  constructor(private authService: AuthService, private router: Router) {}

  onRegister() {
    // Împărțim FullName în două părți: prima parte e firstName, restul e lastName
    const nameParts = this.userData.FullName.trim().split(' ');
    const fName = nameParts[0] || 'Utilizator';
    const lName = nameParts.slice(1).join(' ') || 'PCShop';

    // Construim obiectul exact așa cum îl vrea serverul tău (vezi poza din Swagger)
    const payload = {
      username: this.userData.Email, // Serverul vrea username, punem email-ul că e unic
      email: this.userData.Email,
      password: this.userData.Password,
      firstName: fName,
      lastName: lName
    };

    console.log('Trimitem la server:', payload);

    this.authService.register(payload).subscribe({
      next: () => {
        alert('Cont creat cu succes!');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error('Eroare la register:', err);
        alert('Eroare la server (500). Verifică dacă parola e destul de complexă sau dacă email-ul deja există.');
      }
    });
  }
}