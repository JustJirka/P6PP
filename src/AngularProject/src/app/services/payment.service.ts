import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../enviroments/enviroment';
import { BehaviorSubject } from 'rxjs';
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
  private updatePaymentStatusUrl = `${environment.api.payment}/updatepayment`;
  private balanceSubject = new BehaviorSubject<number | null>(null);
  balance$ = this.balanceSubject.asObservable();


  constructor(private http: HttpClient) {}
  updatePaymentStatus(body: { Id: number; Status: string }): Observable<any> {
  return this.http.post<any>(this.updatePaymentStatusUrl, body).pipe(
    map(response => {
      this.getUserBalance().subscribe();
      return response;
    }),
    catchError(error => {
      console.error('Failed to update payment status', error);
      return throwError(() => error);
    })
  );
}

  getUserBalance(): Observable<number> {
  const id = localStorage.getItem('userId');

  if (!id) {
    return throwError(() => new Error('User ID not found in localStorage'));
  }

  const reqUrl = `${this.getBalanceUrl}/${id}`;

  return this.http.get<any>(reqUrl).pipe(
    map(response => {
      const balance = response.data.creditBalance;
      this.balanceSubject.next(balance);
      return balance;
    }),
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
