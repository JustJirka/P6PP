import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../enviroments/enviroment';

export interface Payment {
  userId: number;
  roleId: number;
  transactionType: string;
  amount: number;
}

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  private getBalanceUrl = `${environment.api.payment}/getbalance`;
  private createPaymentUrl = `${environment.api.payment}/createpayment`;


  constructor(private http: HttpClient) {}

  getUserBalance(): Observable<number> {
    const id = localStorage.getItem('userId');

    if (!id) {
      return throwError(() => new Error('User ID not found in localStorage'));
    }

    const reqUrl = `${this.getBalanceUrl}/${id}`;

    return this.http.get<any>(reqUrl).pipe(
      map(response => response.data.creditBalance),
      catchError(error => {
        console.error('Error in receiving balance:', error);
        return throwError(() => error);
      })
    );
  }

  createPayment(payment: Payment): Observable<any> {
    return this.http.post<any>(this.createPaymentUrl, payment).pipe(
      catchError(error => {
        console.error('Error during payment creation:', error);
        return throwError(() => error);
      })
    );
  }
}
