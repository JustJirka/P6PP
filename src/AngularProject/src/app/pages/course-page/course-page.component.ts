import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavigationComponent } from '../../components/navigation/navigation.component';
import { FooterComponent } from '../../components/footer/footer.component';
import { CourseService } from '../../services/course.service';
import { UserService } from '../../services/user.service';
import { Course } from '../../services/interfaces/course';
import { ToastrService } from 'ngx-toastr'; 
import { PaymentService } from '../../services/payment.service';

@Component({
  selector: 'app-course-page',
  standalone: true,
  imports: [CommonModule, NavigationComponent, FooterComponent],
  templateUrl: './course-page.component.html',
  styleUrls: ['./course-page.component.scss']
})
export class CoursePageComponent {
  constructor(
    private courseService: CourseService,
    private userService: UserService,
    private paymentService: PaymentService,
    private toastr: ToastrService 
  ) {}

  course!: Course;
  trainer: string = '';
  bookingId: number | null = null; // If  NULL - booking is not exists
  isLoading = false;
  balance: number | null = null;

  async ngOnInit() {
    const currentUrl: string = window.location.href;
    const id = this.getLastSegment(currentUrl);
  
    this.courseService.getOneCourse(id).subscribe(response => {
      this.course = response.data;
      this.userService.getUserById(this.course.trainerId).subscribe(trainerResponse => {
        console.log("Trainer response: ", trainerResponse);
        this.trainer = trainerResponse.firstName + ' ' + trainerResponse.lastName;
      });
  
      this.checkIfBooked(this.course.id);

      this.paymentService.balance$.subscribe({
      next: (newBalance) => {
          this.balance = newBalance;
        }
      });

      this.paymentService.getUserBalance().subscribe();
      });
  }

  checkIfBooked(courseId: number) {
    this.courseService.getUserBookings().subscribe(response => {
      const bookings = response.data || [];
      const existingBooking = bookings.find((b: any) => b.serviceId === courseId);

      if (existingBooking) {
        this.bookingId = existingBooking.id;
      } else {
        this.bookingId = null;
      }
    });
  }

  enrollOrCancel() {
    setTimeout(() => {
      this.isLoading = true;

      const userIdString = localStorage.getItem('userId');
      if (!userIdString || isNaN(Number(userIdString))) {
        this.isLoading = false;
        this.toastr.error('Invalid user ID in local storage.', 'Error');
        return;
      }

      const userId = Number(userIdString);

      if (this.bookingId) {
        this.userService.getUserById(userId).subscribe({
          next: user => {
            if (!user) {
              console.error('User object is null or undefined');
              this.isLoading = false;
              this.toastr.error('User not found.', 'Error');
              return;
            }

            this.courseService.cancelBooking(this.bookingId!).subscribe({
              next: () => {
                this.bookingId = null;
                this.checkIfBooked(this.course.id);

                this.paymentService.createPayment({
                  userId: userId,
                  roleId: user.roleId,
                  transactionType: 'credit',
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
              error: err => {
                console.error("Failed to cancel reservation:", err);
                this.toastr.error('Failed to cancel reservation.', 'Error');
                this.isLoading = false;
              }
            });
          },
          error: err => {
            console.error('Error retrieving user info:', err);
            this.isLoading = false;
            this.toastr.error('Error retrieving user info.', 'Error');
          }
        });
      } else {
        this.userService.getUserById(userId).subscribe({
          next: user => {
            if (!user) {
              console.error('User object is null or undefined');
              this.isLoading = false;
              this.toastr.error('User not found.', 'Error');
              return;
            }

            this.courseService.bookService(this.course.id).subscribe({
              next: bookingResponse => {
                const bookingId = bookingResponse?.id;
                this.bookingId = bookingId;

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
                        console.error('Failed to confirm payment status', err);
                      }
                    });

                    this.isLoading = false;
                    this.toastr.success('Enrolled and payment processed successfully!', 'Success');
                    this.checkIfBooked(this.course.id);
                  },
                  error: err => {
                    console.error("Failed to create payment:", err);
                    this.isLoading = false;
                    this.toastr.error('Failed to create payment.', 'Error');
                  }
                });
              },
              error: err => {
                console.error("Failed to enroll:", err);
                this.isLoading = false;
                this.toastr.error(err.error?.message || 'Failed to enroll.', 'Error');
              }
            });
          },
          error: err => {
            console.error('Error retrieving user info:', err);
            this.isLoading = false;
            this.toastr.error('Error retrieving user info.', 'Error');
          }
        });
      }
    }, 500);
  }

  // Example image array
  imageUrls: string[] = [
    'https://goo.su/kSY4URo?',
    'https://tse1.mm.bing.net/th?id=OIG3.uUITmFrj0U3EZKUrsMjq&cb=iwc1&pid=ImgGn',
    'https://images.pexels.com/photos/7045575/pexels-photo-7045575.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=2',
    'https://tse3.mm.bing.net/th?id=OIG2.5bowY.5mQdQKTIv5qY1g&pid=ImgGn',
    'https://images.pexels.com/photos/3839058/pexels-photo-3839058.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=2'

  ];
  currentIndex = 0;

  get currentImage(): string {
    return this.imageUrls[this.currentIndex];
  }

  prevImage(): void {
    this.currentIndex = (this.currentIndex - 1 + this.imageUrls.length) % this.imageUrls.length;
  }

  nextImage(): void {
    this.currentIndex = (this.currentIndex + 1) % this.imageUrls.length;
  }

  private getLastSegment(url: string): string {
    const parts = url.split('/');
    return parts.pop() || '1';
  }
}