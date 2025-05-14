import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { lastValueFrom, Observable, of,BehaviorSubject, Subject, forkJoin, map} from 'rxjs';
import { Course } from './interfaces/course';
import { BookingResponse } from './interfaces/booking';
import { Booking } from './interfaces/booking';
import { environment } from '../../enviroments/enviroment';

@Injectable({ providedIn: 'root' })
export class CourseService {
  private requestAllURL = `${environment.api.course}/services`;
  private requestSingleURL = `${environment.api.course}/services/`;
  private bookingURL = `${environment.api.course}/Bookings`;
  private refreshBookingsSubject = new Subject<void>();
  refreshBookings$ = this.refreshBookingsSubject.asObservable();
  constructor(private http: HttpClient) {}

  getAllCourses(): Observable<any> {
    return this.http.get<Course[]>(this.requestAllURL);
  }

  getActualCourses(coursesArr: Course[]): Course[] {
    const now = new Date();

    return coursesArr.filter(course => {
      const endDate = new Date(course.end);
      return endDate > now && !course.isCancelled;
    });
  }


  getOneCourse(id: any): Observable<any>{
    const reqUrl = this.requestSingleURL + id.toString();
    return this.http.get<Course>(reqUrl);
  }

  notifyRefreshBookings() {
    this.refreshBookingsSubject.next();
  }
  
  filterCources(courcesArr: Course[]) {
    const res: Course[] = courcesArr.filter(course => !course.isCancelled);
    res.sort((a, b) => {
      return new Date(b.start).getTime() - new Date(a.start).getTime();
    });
    
    return res;
  }

  bookService(serviceId: number): Observable<any> {
    const body = { serviceId: serviceId };
    return this.http.post(this.bookingURL, body);
  }

  getUserBookings(): Observable<any> {
    return this.http.get(this.bookingURL);
  }

  // getUserCourses(booking: BookingResponse) {
  //   const res: Course[] = [];

  //   for (const book of booking.data){
  //     this.getOneCourse(book.serviceId.toString()).subscribe(response => {
  //       res.push(response.data)
  //     });
  //   }

  //   res.sort((a, b) => b.start.getTime() - a.start.getTime());
  //   return res;
  // }

  getUserCourses(booking: BookingResponse): Observable<Course[]> {
    const courseObservables = booking.data.map(book =>
      this.getOneCourse(book.serviceId.toString()).pipe(
        map(response => response.data)
      )
    );

    return forkJoin(courseObservables);
  }
  
  cancelBooking(bookingId: number): Observable<any> {
    return this.http.delete(`${this.bookingURL}/${bookingId}`);
  }
}