import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { startWith } from 'rxjs';
import { RoomService } from '../../core/services/room.service';
import { BookingService } from '../../core/services/booking.service';
import { LocationService } from '../../core/services/location.service';
import { NotificationService } from '../../core/services/notification.service';
import { Room } from '../../core/models/room.model';
import { Booking } from '../../core/models/booking.model';
import { Branch } from '../../core/models/location.model';

@Component({
  selector: 'app-booking',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule],
  templateUrl: './booking.component.html',
  styleUrls: ['./booking.component.scss']
})
export class BookingComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private roomService = inject(RoomService);
  private bookingService = inject(BookingService);
  private locationService = inject(LocationService);
  private notify = inject(NotificationService);

  room = signal<Room | null>(null);
  selectedBranch = signal<Branch | null>(null);
  isLoading = signal(true);
  isSubmitting = signal(false);
  confirmedBooking = signal<Booking | null>(null);
  step = signal<1 | 2 | 3>(1);

  readonly today = new Date().toISOString().split('T')[0];

  form = this.fb.group({
    checkInDate: ['', Validators.required],
    checkOutDate: ['', Validators.required],
    numberOfGuests: [1, [Validators.required, Validators.min(1)]],
    specialRequests: ['']
  });

  // Convert form.valueChanges to a signal so computed() can track it reactively.
  // startWith emits the initial value so we don't get a stale snapshot.
  private readonly formValues = toSignal(
    this.form.valueChanges.pipe(startWith(this.form.value)),
    { initialValue: this.form.value }
  );

  nightsCount = computed(() => {
    const { checkInDate: ci, checkOutDate: co } = this.formValues();
    if (!ci || !co) return 0;
    const diff = (new Date(co).getTime() - new Date(ci).getTime()) / 86400000;
    return diff > 0 ? Math.floor(diff) : 0;
  });

  subtotal = computed(() =>
    (this.room()?.pricePerNight ?? 0) * this.nightsCount()
  );

  tax = computed(() => Math.round(this.subtotal() * 0.12 * 100) / 100);
  total = computed(() => this.subtotal() + this.tax());

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('roomId'));
    const locationId = this.route.snapshot.queryParamMap.get('locationId');

    this.roomService.getRoom(id).subscribe({
      next: r => { this.room.set(r); this.isLoading.set(false); },
      error: () => { this.notify.error('Room not found.'); this.router.navigate(['/rooms']); }
    });

    if (locationId) {
      this.locationService.getBranch(+locationId).subscribe({
        next: b => this.selectedBranch.set(b),
        error: () => {}
      });
    }
  }

  nextStep(): void {
    if (!this.form.get('checkInDate')?.value || !this.form.get('checkOutDate')?.value) {
      this.form.markAllAsTouched();
      return;
    }
    if (this.nightsCount() <= 0) {
      this.notify.error('Check-out date must be after check-in date.');
      return;
    }
    this.step.set(2);
  }

  submitBooking(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.isSubmitting.set(true);

    const val = this.form.value;
    this.bookingService.createBooking({
      roomId: this.room()!.id,
      checkInDate: val.checkInDate!,
      checkOutDate: val.checkOutDate!,
      numberOfGuests: val.numberOfGuests!,
      specialRequests: val.specialRequests || undefined,
      locationId: this.selectedBranch()?.id
    }).subscribe({
      next: booking => {
        this.confirmedBooking.set(booking);
        this.step.set(3);
        this.notify.success('Booking confirmed! Check your dashboard for details.');
        this.isSubmitting.set(false);
      },
      error: err => {
        this.notify.error(err?.error?.message || 'Booking failed. Please try again.');
        this.isSubmitting.set(false);
      }
    });
  }
}
