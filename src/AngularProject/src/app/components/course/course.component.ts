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
        this.toastr.error('Invalid user. Please log in again.', 'Error');
        return;
      }

      const userId = Number(userIdString);

      this.userService.getUserById(userId).subscribe({
        next: user => {
          if (this.isBooked && this.bookingId) {
            // Cancel existing booking only (no rebook)
            this.courseService.cancelBooking(this.bookingId).subscribe({
              next: () => {
                this.bookingId = null;
                this.isBooked = false;
                this.checkIfBooked(this.course.id);

                // Optionally record credit/refund
                this.paymentService.createPayment({
                  userId: userId,
                  roleId: user.roleId,
                  transactionType: 'credit', // Represents refund/credit
                  amount: this.course.price
                }).subscribe({
                  next: response => {
                    this.paymentService.updatePaymentStatus({
                      Id: response.data,
                      Status: 'confirm'
                    }).subscribe({
                      next: () => {
                        console.log('Credit payment status confirmed');
                      },
                      error: err => {
                        console.error('Failed to confirm credit payment status', err);
                      }
                    });

                    this.toastr.info('Reservation cancelled and credit issued.', 'Cancelled');
                    this.courseService.notifyRefreshBookings();
                    this.isLoading = false;
                  },
                  error: err => {
                    console.error("Failed to issue credit:", err);
                    this.toastr.error('Cancellation succeeded, but credit failed.', 'Partial Success');
                    this.courseService.notifyRefreshBookings();
                    this.isLoading = false;
                  }
                });
              },
              error: () => {
                this.toastr.error('Failed to cancel reservation.', 'Error');
                this.isLoading = false;
              }
            });
          } else {
            // Make a new reservation
            this.courseService.bookService(this.course.id).subscribe({
              next: bookingResponse => {
                this.bookingId = bookingResponse?.id;
                this.isBooked = true;
                this.checkIfBooked(this.course.id);

                this.paymentService.createPayment({
                  userId: userId,
                  roleId: user.roleId,
                  transactionType: 'reservation',
                  amount: this.course.price
                }).subscribe({
                  next: response => {
                    this.paymentService.updatePaymentStatus({
                      Id: response.data,
                      Status: 'confirm'
                    }).subscribe({
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
                    console.error("Payment failed:", err);
                    this.toastr.error('Payment failed.', 'Error');
                    this.isLoading = false;
                  }
                });
              },
              error: err => {
                console.error("Failed to reserve course:", err);
                this.toastr.error('Failed to reserve course.', 'Error');
                this.isLoading = false;
              }
            });
          }
        },
        error: err => {
          console.error("Failed to load user data:", err);
          this.toastr.error('Failed to load user data.', 'Error');
          this.isLoading = false;
        }
      });
    }, 500);
  }
}