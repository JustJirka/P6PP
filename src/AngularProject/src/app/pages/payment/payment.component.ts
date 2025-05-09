import { Component } from '@angular/core';
import { NavigationComponent } from "../../components/navigation/navigation.component";
import { PaymentService } from '../../services/payment.service';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-payment',
  standalone: true,
  imports: [NavigationComponent],
  templateUrl: './payment.component.html',
  styleUrl: './payment.component.scss'
})
export class PaymentComponent {
  paymentAmount = '0,0CZK';
  private numericAmount = 0;

  constructor(
    private paymentService: PaymentService,
    private userService: UserService
  ) {}

  onInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    const rawValue = input.value.replace(/[^\d]/g, '');
    this.numericAmount = rawValue ? parseInt(rawValue, 10) : 0;

    const newValue = this.numericAmount > 0 ? `${this.numericAmount} CZK` : '0,0CZK';
    this.paymentAmount = newValue;
  }

  setFixedAmount(amount: number): void {
    this.numericAmount = amount;
    this.paymentAmount = `${this.numericAmount} CZK`;

    const input = document.getElementById('amountInput') as HTMLInputElement;
    if (input) {
      input.value = this.paymentAmount;
      input.focus();
      input.setSelectionRange(this.paymentAmount.length, this.paymentAmount.length);
    }
  }

  depositMoney(): void {
    const userIdString = localStorage.getItem('userId');

    if (!userIdString) {
      console.error('userId not found in local storage');
      return;
    }

    const userId = Number(userIdString);
    if (isNaN(userId)) {
      console.error('Invalid userId');
      return;
    }

    this.userService.getUserById(userId).subscribe({
      next: user => {
        if (!user) {
          console.error('User not found');
          return;
        }

        const roleId = user.roleId;

        this.paymentService.createPayment({
          userId: userId,
          roleId: roleId,
          transactionType: 'credit',
          amount: this.numericAmount
        }).subscribe({
          next: response => {
            console.log('Payment successfully created:', response);
        
            if (response && response.data) {
              this.paymentService.updatePaymentStatus({ Id: response.data, Status: 'confirm' }).subscribe({
                next: () => {
                  console.log('Payment status confirmed');
                },
                error: err => {
                  console.error('Error confirming payment status:', err);
                }
              });
            }
          },
          error: err => {
            console.error('Error during payment creation:', err);
          }
        });
      },
      error: err => {
        console.error('Error on user retrieval:', err);
      }
    });
  }
}
