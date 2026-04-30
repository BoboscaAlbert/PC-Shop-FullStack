import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem('token');

  // Adăugăm token-ul doar dacă există și nu suntem pe rutele de login/register
  if (token && token !== 'null' && token !== 'undefined' && !req.url.includes('/Auth/')) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token.replace(/"/g, '')}` // Eliminăm ghilimelele extra
      }
    });
  }

  return next(req);
};