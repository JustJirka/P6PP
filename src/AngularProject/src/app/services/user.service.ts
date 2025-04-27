import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

interface UpdateUserRequest {
  username?: string;
  firstName?: string;
  lastName?: string;
  email?: string;
  phoneNumber?: string;
  weight?: number;
  height?: number;
  sex?: string;
  dateOfBirth?: string;
}

@Injectable({ providedIn: 'root' })
export class UserService {
  private userBaseUrl = 'http://localhost:5189/api/user';
  private authBaseUrl = 'http://localhost:8005/api/auth';

  constructor(private http: HttpClient) {}

  getStoredUserId(): number | null {
    const rawId = localStorage.getItem('userId');
    return rawId ? parseInt(rawId, 10) : null;
  }

  getCurrentUser(): Observable<any> {
    const id = this.getStoredUserId();
    if (!id) {
      console.warn('❌ no userId localStorage');
      return of(null);
    }
    return this.getUserById(id);
  }

  getUserById(id: number): Observable<any> {
    return this.http.get<any>(`${this.userBaseUrl}/${id}`).pipe(
      map(response => response?.data?.user || null),
      catchError(err => {
        console.error('🧨 Error was occured:', err);
        return of(null);
      })
    );
  }

  updateUser(id: number, payload: UpdateUserRequest): Observable<any> {
    return this.http.put(`${this.userBaseUrl}/${id}`, payload);
  }

  // resetPassword(userId: number, newPassword: string): Observable<any> {
  //   return this.http.post(`${this.authBaseUrl}/reset-password`, {
  //     userId,
  //     newPassword
  //   });
  // }

  changePassword(newPassword: string): Observable<any> {
    const token = localStorage.getItem('token');
    return this.http.post(`${this.authBaseUrl}/change-password`, {
      newPassword,
      token
    });
  }

  deleteUser(id: number): Observable<any> {
    return this.http.delete(`${this.userBaseUrl}/${id}`);
  }
}