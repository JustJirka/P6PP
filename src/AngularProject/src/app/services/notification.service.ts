import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { UserService } from './user.service';
import { environment } from '../../enviroments/enviroment';

@Injectable({
  providedIn: 'root'
})

export class NotificationService {
  private apiUrl = environment.api.notification;
  private notificationsSubject = new BehaviorSubject<any[]>([]);
  public notifications$ = this.notificationsSubject.asObservable();

  constructor(
    private http: HttpClient,
    private userService: UserService
  ) { }

  public loadNotifications(): void {
    const userId = this.userService.getStoredUserId();
  
    if (userId === null) {
      this.notificationsSubject.next([]);
      return;
    }
  
    const url = `${this.apiUrl}/logs/getallnotifications/${userId}`;
  
    const params = new HttpParams()
      .set('unreadOnly', 'true')
      .set('perPage', '10')
      .set('page', '1');
  
    this.http.get<any>(url, { params })
      .subscribe(response => {
        const logs = response?.data?.notificationLogs ?? [];
        this.notificationsSubject.next(logs);
      });
  }
}
