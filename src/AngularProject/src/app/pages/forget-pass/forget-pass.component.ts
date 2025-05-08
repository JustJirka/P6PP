import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { environment } from '../../../enviroments/enviroment';

@Component({
  selector: 'app-forget-pass',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './forget-pass.component.html',
  styleUrl: './forget-pass.component.scss'
})
export class ForgetPassComponent {
  resetForm: FormGroup;
  resetError: string | null = null;
  successMessage: string | null = null;
  showVerifyMessage: boolean = false;
  private authUrl = environment.api.auth

  constructor(
    private fb: FormBuilder,
    private http: HttpClient
  ) {
    this.resetForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
    });
  }

  onSubmit() {
    this.resetError = null;
    this.successMessage = null;
  
    if (this.resetForm.valid) {
      const email = this.resetForm.get('email')?.value;
  
      console.log('Sending request to:', this.authUrl + '/reset-password');
      console.log('With body:', { email });

      this.http.get(this.authUrl + '/reset-password?email=' + email)
        .subscribe({
          next: (response) => {
            console.log('Response from backend:', response); 
            this.successMessage = 'A reset link has been sent to your email.';
            this.showVerifyMessage = true;
            this.resetForm.reset();
          },
          error: (err) => {
            console.error('Error sending request:', err); 
            this.resetError = err?.error?.message || 'Something went wrong. Please try again.';
          }
        });
    } else {
      this.resetError = 'Please enter a valid email.';
    }
  }
  
}
