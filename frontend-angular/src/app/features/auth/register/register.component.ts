import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';

function passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
  const pw = control.get('password')?.value;
  const cpw = control.get('confirmPassword')?.value;
  return pw && cpw && pw !== cpw ? { passwordMismatch: true } : null;
}

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrls: ['../login/login.component.scss', './register.component.scss']
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private notify = inject(NotificationService);
  private router = inject(Router);

  isLoading = signal(false);
  showPassword = signal(false);
  showConfirm = signal(false);
  serverError = signal('');

  form = this.fb.group({
    fullName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: ['', [Validators.pattern(/^\+?[\d\s\-()]{7,20}$/)]],
    password: ['', [Validators.required, Validators.minLength(8),
      Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/)
    ]],
    confirmPassword: ['', Validators.required]
  }, { validators: passwordMatchValidator });

  get f() { return this.form.controls; }

  getPasswordStrength(): { label: string; pct: number; color: string } {
    const pw = this.f.password.value ?? '';
    let score = 0;
    if (pw.length >= 8) score++;
    if (/[A-Z]/.test(pw)) score++;
    if (/[a-z]/.test(pw)) score++;
    if (/\d/.test(pw)) score++;
    if (/[^a-zA-Z\d]/.test(pw)) score++;
    const map = [
      { label: '', pct: 0, color: '' },
      { label: 'Very Weak', pct: 20, color: '#ef4444' },
      { label: 'Weak', pct: 40, color: '#f97316' },
      { label: 'Fair', pct: 60, color: '#eab308' },
      { label: 'Strong', pct: 80, color: '#22c55e' },
      { label: 'Very Strong', pct: 100, color: '#10b981' }
    ];
    return map[score];
  }

  onSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.isLoading.set(true);
    this.serverError.set('');

    this.authService.register(this.form.value as any).subscribe({
      next: res => {
        this.notify.success(`Welcome, ${res.fullName}! Your account has been created.`);
        this.router.navigate(['/dashboard']);
      },
      error: err => {
        this.serverError.set(err?.error?.message || 'Registration failed. Please try again.');
        this.isLoading.set(false);
      }
    });
  }
}
