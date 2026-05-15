import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { UserService } from '../../../core/services/user.service';
import { BookingService } from '../../../core/services/booking.service';
import { DashboardStats } from '../../../core/models/user.model';
import { Booking } from '../../../core/models/booking.model';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, DatePipe],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.scss']
})
export class AdminDashboardComponent implements OnInit {
  private userService = inject(UserService);
  private bookingService = inject(BookingService);

  stats = signal<DashboardStats | null>(null);
  recentBookings = signal<Booking[]>([]);
  isLoading = signal(true);
  readonly today = new Date();

  ngOnInit(): void {
    this.userService.getDashboardStats().subscribe({
      next: s => { this.stats.set(s); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
    this.bookingService.getAllBookings(1, 5).subscribe({
      next: r => this.recentBookings.set(r.items as Booking[])
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

  getBarWidth(amount: number): number {
    const items = this.stats()?.revenueChart ?? [];
    const max = Math.max(...items.map(i => i.amount), 1);
    return Math.round((amount / max) * 100);
  }
}
