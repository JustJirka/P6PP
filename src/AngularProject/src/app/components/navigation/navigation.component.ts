import { Component } from '@angular/core';
import { RouterModule, Router } from '@angular/router'; 
import { ToastrService } from 'ngx-toastr'; 
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { NotificationsComponent } from '../notifications/notifications.component';
import { PaymentService } from '../../services/payment.service';
@Component({
  selector: 'app-navigation',
  imports: [RouterModule, MatIconModule, CommonModule, NotificationsComponent],
  templateUrl: './navigation.component.html',
  styleUrl: './navigation.component.scss'
})
export class NavigationComponent {
  isMenuOpen = false;
  isLoggedIn = false;
  userId: number | null | undefined;
  balance: number | null = null;

  constructor(
    private authService: AuthService,
    private toastr: ToastrService,
    private router: Router,
    private paymentService: PaymentService
  ) {}

    ngOnInit() {
    this.authService.isLoggedIn$.subscribe(status => {
      this.isLoggedIn = status;
      this.userId = this.authService.getUserId();
    });

    this.paymentService.balance$.subscribe({
      next: (newBalance) => {
        this.balance = newBalance;
      }
    });

    this.paymentService.getUserBalance().subscribe();
  }

  logout() {
    this.authService.logout();
    this.toastr.success('You have successfully logged out', 'Goodbye!');
    this.router.navigate(['/']); 
  }

  showNotifications = false;

  toggleNotifications() {
    this.showNotifications = !this.showNotifications;
  }
}