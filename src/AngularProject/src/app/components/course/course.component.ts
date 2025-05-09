import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Course } from '../../services/interfaces/course';
import { CourseService } from '../../services/course.service';
import { UserService } from '../../services/user.service';
import { ToastrService } from 'ngx-toastr';
import { RouterModule } from '@angular/router';
import { PaymentService } from '../../services/payment.service';


@Component({
  selector: 'app-course',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './course.component.html',
  styleUrls: ['./course.component.scss']
})
export class CourseComponent implements OnInit {
  @Input() course!: Course;
  trainerName: string = '';
  isBooked: boolean = false;
  isLoading: boolean = false;
  bookingId: number | null = null;

  constructor(
    private courseService: CourseService,
    private userService: UserService,
    private toastr: ToastrService,
    private paymentService: PaymentService
  ) {}

  ngOnInit(): void {
    if (this.course?.trainerId) {
      this.userService.getUserById(this.course.trainerId).subscribe({
        next: (res) => {
          this.trainerName = `${res.firstName} ${res.lastName}`;
        },
        error: () => {
          this.toastr.error('Failed to load trainer info', 'Error');
        }
      });
    }

    this.checkIfBooked(this.course.id);
  }

  checkIfBooked(courseId: number) {
    this.courseService.getUserBookings().subscribe({
      next: (res) => {
        const bookings = res.data || [];
        const existingBooking = bookings.find(
          (b: any) => b.serviceId === courseId
        );
        if (existingBooking) {
          this.isBooked = true;
          this.bookingId = existingBooking.id;
        } else {
          this.isBooked = false;
          this.bookingId = null;
        }
      }
    });
  }

  toggleReservation(): void {
  setTimeout(() => {
    this.isLoading = true;

    const userIdString = localStorage.getItem('userId');
    if (!userIdString || isNaN(Number(userIdString))) {
      this.isLoading = false;
      this.toastr.error('Invalid user ID. Please log in.', 'Error');
      return;
    }

    const userId = Number(userIdString);

    if (this.isBooked && this.bookingId) {
 
      this.courseService.cancelBooking(this.bookingId).subscribe({
        next: () => {
     
          this.checkIfBooked(this.course.id);
          this.bookingId = null;
          this.toastr.info('Reservation cancelled.', 'Cancelled');
          this.courseService.notifyRefreshBookings();
          this.isLoading = false;
        },
        error: () => {
          this.toastr.error('Failed to cancel reservation.', 'Error');
          this.isLoading = false;
        }
      });
    } else {
     
      this.userService.getUserById(userId).subscribe({
        next: user => {
          this.courseService.bookService(this.course.id).subscribe({
            next: (bookingResponse) => {
              const bookingId = bookingResponse?.id;
              this.bookingId = bookingId;
              this.isBooked = true;
              this.checkIfBooked(this.course.id);
          
              this.paymentService.createPayment({
                userId: userId,
                roleId: user.roleId,
                transactionType: 'reservation',
                amount: this.course.price
              }).subscribe({
                next: response => {
                  
                  this.paymentService.updatePaymentStatus({ Id: response.data, Status: 'confirm' }).subscribe({
                    next: () => {
                      console.log('Payment status set to confirm');
                    },
                    error: err => {
                      console.error('Failed to update payment status to confirm', err);
                    }
                  });
                  
                  this.toastr.success('Course reserved and payment processed!', 'Success');
                  this.courseService.notifyRefreshBookings();
                  this.isLoading = false;
                },
                error: err => {
                  this.toastr.error('Payment failed.', 'Error');
                  this.isLoading = false;
                }
              });
            },
            error: err => {
              this.toastr.error('Failed to reserve course.', 'Error');
              this.isLoading = false;
            }
          });
        },
        error: err => {
          this.toastr.error('Failed to load user data.', 'Error');
          this.isLoading = false;
        }
      });
    }
  }, 500);
}

}
