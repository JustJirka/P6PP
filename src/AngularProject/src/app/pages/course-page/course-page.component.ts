import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavigationComponent } from '../../components/navigation/navigation.component';
import { FooterComponent } from '../../components/footer/footer.component';
import { CourseService } from '../../services/course.service';
import { UserService } from '../../services/user.service';
import { Course } from '../../services/interfaces/course';
import { ToastrService } from 'ngx-toastr'; 

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
    private toastr: ToastrService 
  ) {}

  course!: Course;
  trainer: string = '';
  bookingId: number | null = null; // If  NULL - booking is not exists
  isLoading = false;

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
      if (this.bookingId) {
        this.courseService.cancelBooking(this.bookingId).subscribe({
          next: () => {
            this.isLoading = false;
            this.toastr.success('Booking cancelled successfully!', 'Success');
            this.bookingId = null;
          },
          error: (err) => {
            this.isLoading = false;
            this.toastr.error(err.error?.message || 'Failed to cancel booking.', 'Error');
          }
        });
      } else {
        this.courseService.bookService(this.course.id).subscribe({
          next: (response) => {
            this.isLoading = false;
            this.toastr.success('Enrolled successfully!', 'Success');
            this.checkIfBooked(this.course.id);
          },
          error: (err) => {
            this.isLoading = false;
            if(err.error?.message == "Invalid JWT: UserId claim is missing.") this.toastr.error("You should be logged in to make a reservation.", 'Error');
            else this.toastr.error(err.error?.message || 'Failed to enroll.', 'Error');
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