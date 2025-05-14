import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

import { NavigationComponent } from '../../components/navigation/navigation.component';
import { FooterComponent } from '../../components/footer/footer.component';

import { CourseService } from '../../services/course.service';
import { UserService } from '../../services/user.service';
import { ToastrService } from 'ngx-toastr';

import { BookingResponse } from '../../services/interfaces/booking';
import { Course } from '../../services/interfaces/course';
import { CourseComponent } from "../../components/course/course.component";

@Component({
  selector: 'app-profile.page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NavigationComponent, FooterComponent, CourseComponent],
  templateUrl: './profile.page.html',
  styleUrl: './profile.page.scss'
})
export class ProfilePage implements OnInit {
  user: any;
  bookings: Course[] = [];

  showEditForm = false;
  showSettingsForm = false;
  hidePassword = true;
  hideRepeatPassword = true;
  editForm!: FormGroup;
  settingsForm!: FormGroup;

  constructor(
    private userService: UserService,
    private fb: FormBuilder,
    private toastr: ToastrService,
    private courseService: CourseService
  ) {}

  ngOnInit() {
    this.userService.getCurrentUser().subscribe((user) => {
      this.user = user;
      console.log("User: ", user)
      this.initForms();
    });

    this.courseService.getUserBookings().subscribe((bookingResponse) => {
      this.courseService.getUserCourses(bookingResponse).subscribe((courses: Course[]) => {
        this.bookings = courses.filter(course =>
          new Date(course.end).getTime() > Date.now() && !course.isCancelled
        );
      });
    });
  }

  initForms() {
    this.editForm = this.fb.group({
      username: [this.user?.username || ''],
      firstName: [this.user?.firstName || ''],
      lastName: [this.user?.lastName || ''],
      email: [this.user?.email || ''],
      phoneNumber: [this.user?.phoneNumber || ''],
      weight: [this.user?.weight || ''],
      height: [this.user?.height || ''],
      sex: [this.user?.sex || ''],
      // dateOfBirth: [this.user?.dateOfBirth ? this.user.dateOfBirth.split('T')[0] : '']
    });

    this.settingsForm = this.fb.group({
      newPassword: ['', [
        Validators.required, 
        Validators.minLength(8),
        Validators.pattern('^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[^A-Za-z0-9]).{8,}$')
      ]],
      repeatPassword: ['', Validators.required]
    });
  }

  toggleEditForm() {
    this.showEditForm = !this.showEditForm;
    if (this.showEditForm) this.showSettingsForm = false;
  }

  toggleSettingsForm() {
    this.showSettingsForm = !this.showSettingsForm;
    if (this.showSettingsForm) this.showEditForm = false;
  }

  onSaveProfile() {
    const raw = this.editForm.value;
    const cleaned: any = {};
    for (const key in raw) {
      if (raw[key] !== null && raw[key] !== '' && raw[key].toString().trim() !== '') {
        cleaned[key] = raw[key];
      }
    }
    this.userService.updateUser(this.user.id, cleaned).subscribe(() => {
      this.showEditForm = false;
      this.ngOnInit();
      this.toastr.success('Profile updated successfully!');
    });
  }

  onChangePassword() {
    if (this.settingsForm.invalid) return;

    const newPassword = this.settingsForm.value.newPassword;
    const repeatPassword = this.settingsForm.value.repeatPassword;
    if (newPassword && repeatPassword) {
      this.userService.changePassword(newPassword, repeatPassword).subscribe(() => {
        this.showSettingsForm = false;
        this.toastr.success('Password reset successfully!');
      });
    }
  } 

  getAge(birthDate: string): number {
    const dob = new Date(birthDate);
    const diff = Date.now() - dob.getTime();
    const ageDate = new Date(diff); 
    return Math.abs(ageDate.getUTCFullYear() - 1970);
  }

  // onDeleteAccount() {
  //   const confirmDelete = window.confirm('Are you sure you want to send a request to delete your account? This action is irreversible.'); // Are you sure you want to delete your account? This action is irreversible.
  //   if (confirmDelete) {
  //     this.userService.deleteUser(this.user.id).subscribe(() => {
  //       localStorage.removeItem('token');
  //       this.toastr.success('Account deleted.');
  //       window.location.href = '/';
  //     });
  //   }
  // }
}
