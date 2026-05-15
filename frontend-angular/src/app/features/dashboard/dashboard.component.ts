import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from '../../core/services/user.service';
import { BookingService } from '../../core/services/booking.service';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';
import { User, UpdateUser } from '../../core/models/user.model';
import { Booking } from '../../core/models/booking.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule, DatePipe],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  private userService = inject(UserService);
  private bookingService = inject(BookingService);
  private authService = inject(AuthService);
  private notify = inject(NotificationService);
  private fb = inject(FormBuilder);

  user = signal<User | null>(null);
  bookings = signal<Booking[]>([]);
  isLoading = signal(true);
  activeTab = signal<'bookings' | 'profile'>('bookings');
  isEditingProfile = signal(false);
  isSavingProfile = signal(false);

  profileForm = this.fb.group({
    fullName: ['', [Validators.required, Validators.minLength(2)]],
    phoneNumber: [''],
    address: [''],
    city: [''],
    state: [''],
    zipCode: ['']
  });

  ngOnInit(): void {
    this.userService.getProfile().subscribe({
      next: u => {
        this.user.set(u);
        this.profileForm.patchValue({
          fullName: u.fullName,
          phoneNumber: u.phoneNumber ?? '',
          address: u.address ?? '',
          city: u.city ?? '',
          state: u.state ?? '',
          zipCode: u.zipCode ?? ''
        });
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });

    this.bookingService.getMyBookings().subscribe({
      next: b => this.bookings.set(b)
    });
  }

  saveProfile(): void {
    if (this.profileForm.invalid) { this.profileForm.markAllAsTouched(); return; }
    this.isSavingProfile.set(true);
    const dto = this.profileForm.value as UpdateUser;
    this.userService.updateProfile(dto).subscribe({
      next: u => {
        this.user.set(u);
        this.isEditingProfile.set(false);
        this.isSavingProfile.set(false);
        this.notify.success('Profile updated successfully!');
      },
      error: () => {
        this.notify.error('Failed to update profile.');
        this.isSavingProfile.set(false);
      }
    });
  }

  cancelBooking(id: number): void {
    this.bookingService.cancelBooking(id).subscribe({
      next: () => {
        this.notify.success('Booking cancelled.');
        this.bookingService.getMyBookings().subscribe(b => this.bookings.set(b));
      },
      error: err => this.notify.error(err?.error?.message || 'Cancel failed.')
    });
  }

  getStatusClass(s: string): string {
    const m: Record<string, string> = {
      Confirmed: 'badge-success', Pending: 'badge-warning',
      Cancelled: 'badge-error', CheckedIn: 'badge-info',
      CheckedOut: 'badge-info', NoShow: 'badge-error'
    };
    return m[s] ?? 'badge-info';
  }

  get upcomingBookings(): Booking[] {
    return this.bookings().filter(b =>
      b.status === 'Confirmed' || b.status === 'Pending'
    ).sort((a, b) => new Date(a.checkInDate).getTime() - new Date(b.checkInDate).getTime());
  }

  get pastBookings(): Booking[] {
    return this.bookings().filter(b =>
      b.status === 'CheckedOut' || b.status === 'Cancelled' || b.status === 'NoShow'
    ).sort((a, b) => new Date(b.checkInDate).getTime() - new Date(a.checkInDate).getTime());
  }

  get totalSpent(): number {
    return this.bookings()
      .filter(b => b.status !== 'Cancelled')
      .reduce((sum, b) => sum + b.totalAmount, 0);
  }
}
