import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private notify = inject(NotificationService);
  private router = inject(Router);

  isLoading = signal(false);
  showPassword = signal(false);
  serverError = signal('');

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  get email() { return this.form.get('email')!; }
  get password() { return this.form.get('password')!; }

  onSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.isLoading.set(true);
    this.serverError.set('');

    this.authService.login(this.form.value as any).subscribe({
      next: res => {
        this.notify.success(`Welcome back, ${res.fullName}!`);
        this.router.navigate([res.role === 'Admin' ? '/admin/dashboard' : '/dashboard']);
      },
      error: err => {
        this.serverError.set(err?.error?.message || 'Invalid email or password.');
        this.isLoading.set(false);
      }
    });
  }

  togglePassword(): void { this.showPassword.update(v => !v); }
}
