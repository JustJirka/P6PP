import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { RouterModule } from '@angular/router';
import { environment } from '../../../enviroments/enviroment';

@Component({
  selector: 'app-reset-pass',
  imports: [ReactiveFormsModule, CommonModule, RouterModule],
  templateUrl: './reset-pass.component.html',
  styleUrl: './reset-pass.component.scss'
})
export class ResetPassComponent {
  resetForm: FormGroup;
  hidePassword = true;
  hideRepeatPassword = true;
  tryToLogin: boolean = false;
  private authUrl = environment.api.auth;
  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private http: HttpClient
  ) {
    this.resetForm = this.fb.group({ 
      newPassword: ['', [
        Validators.required, 
        Validators.minLength(8),
        Validators.pattern('^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[^A-Za-z0-9]).{8,}$')]],

      repeatPassword: ['', [
        Validators.required]]
    });
  }

  onSubmit() {  
    if (this.resetForm.valid) {
      const form = this.resetForm.value;


      this.route.queryParams.subscribe(params => {
        const userId = params['userId'];
        const token = params['token'];
        console.log(params);

        const payload = {
          newPassword: form.newPassword,
          token: token,
          userId: userId
        };
        console.log("reset password payload", payload)

        this.http.post(`${this.authUrl}` + '/reset-password', payload)
        .subscribe({
          next: () => {alert('Your password has been changed'),
            this.tryToLogin = true;
          },
          error: (err) => {
            console.error(err);
            alert('Error: ' + (err.error?.message || 'unexpected error'));
          }
        });
      });
    }
  }

}
