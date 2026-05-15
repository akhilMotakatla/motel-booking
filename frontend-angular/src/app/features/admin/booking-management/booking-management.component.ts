import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { BookingService } from '../../../core/services/booking.service';
import { NotificationService } from '../../../core/services/notification.service';
import { Booking, BookingStatus } from '../../../core/models/booking.model';
import { PagedResult } from '../../../core/models/room.model';

@Component({
  selector: 'app-booking-management',
  standalone: true,
  imports: [CommonModule, DatePipe],
  templateUrl: './booking-management.component.html',
  styleUrls: ['../room-management/room-management.component.scss', './booking-management.component.scss']
})
export class BookingManagementComponent implements OnInit {
  private bookingService = inject(BookingService);
  private notify = inject(NotificationService);

  result = signal<PagedResult<Booking> | null>(null);
  isLoading = signal(true);
  selectedStatus = signal<string>('');
  currentPage = signal(1);

  readonly statuses = ['', 'Pending', 'Confirmed', 'CheckedIn', 'CheckedOut', 'Cancelled', 'NoShow'];

  ngOnInit(): void { this.load(); }

  load(): void {
    this.isLoading.set(true);
    this.bookingService.getAllBookings(this.currentPage(), 15, this.selectedStatus() || undefined).subscribe({
      next: r => { this.result.set(r); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  filterByStatus(s: string): void { this.selectedStatus.set(s); this.currentPage.set(1); this.load(); }
  changePage(p: number): void { this.currentPage.set(p); this.load(); }

  updateStatus(bookingId: number, status: BookingStatus): void {
    this.bookingService.updateStatus(bookingId, status).subscribe({
      next: () => { this.notify.success(`Booking status updated to ${status}.`); this.load(); },
      error: () => this.notify.error('Update failed.')
    });
  }

  cancelBooking(id: number): void {
    this.bookingService.cancelBooking(id).subscribe({
      next: () => { this.notify.success('Booking cancelled.'); this.load(); },
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

  get bookings(): Booking[] { return this.result()?.items ?? []; }
  get pages(): number[] {
    return Array.from({ length: this.result()?.totalPages ?? 1 }, (_, i) => i + 1);
  }
}
