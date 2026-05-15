import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RoomService } from '../../../core/services/room.service';
import { NotificationService } from '../../../core/services/notification.service';
import { Room } from '../../../core/models/room.model';

@Component({
  selector: 'app-room-management',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './room-management.component.html',
  styleUrls: ['./room-management.component.scss']
})
export class RoomManagementComponent implements OnInit {
  private roomService = inject(RoomService);
  private notify = inject(NotificationService);
  private fb = inject(FormBuilder);

  rooms = signal<Room[]>([]);
  isLoading = signal(true);
  showModal = signal(false);
  editingRoom = signal<Room | null>(null);
  isSubmitting = signal(false);
  deleteConfirmId = signal<number | null>(null);

  readonly roomTypes = [
    { id: 1, name: 'Standard' }, { id: 2, name: 'Deluxe' },
    { id: 3, name: 'Suite' }, { id: 4, name: 'Executive' }, { id: 5, name: 'Family' }
  ];

  form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(150)]],
    description: ['', [Validators.required, Validators.maxLength(2000)]],
    pricePerNight: [null as number | null, [Validators.required, Validators.min(1)]],
    maxOccupancy: [2, [Validators.required, Validators.min(1), Validators.max(20)]],
    floorNumber: [1, [Validators.required, Validators.min(1)]],
    roomNumber: ['', [Validators.required]],
    sizeInSqFt: [null as number | null, [Validators.required, Validators.min(1)]],
    roomTypeId: [null as number | null, Validators.required],
    thumbnailUrl: [''],
    isFeatured: [false]
  });

  ngOnInit(): void { this.loadRooms(); }

  loadRooms(): void {
    this.isLoading.set(true);
    this.roomService.getRooms({ pageNumber: 1, pageSize: 100 }).subscribe({
      next: r => { this.rooms.set(r.items as Room[]); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  openAdd(): void {
    this.editingRoom.set(null);
    this.form.reset({ maxOccupancy: 2, floorNumber: 1, isFeatured: false });
    this.showModal.set(true);
  }

  openEdit(room: Room): void {
    this.editingRoom.set(room);
    this.form.patchValue({
      name: room.name, description: room.description,
      pricePerNight: room.pricePerNight, maxOccupancy: room.maxOccupancy,
      floorNumber: room.floorNumber, roomNumber: room.roomNumber,
      sizeInSqFt: room.sizeInSqFt, roomTypeId: room.roomTypeId,
      thumbnailUrl: room.thumbnailUrl ?? '', isFeatured: room.isFeatured
    });
    this.showModal.set(true);
  }

  closeModal(): void { this.showModal.set(false); this.editingRoom.set(null); this.form.reset(); }

  submit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.isSubmitting.set(true);
    const val = this.form.value as any;
    const dto = { ...val, amenityIds: [], imageUrls: val.thumbnailUrl ? [val.thumbnailUrl] : [] };

    const obs = this.editingRoom()
      ? this.roomService.updateRoom(this.editingRoom()!.id, { ...dto, status: 'Available' })
      : this.roomService.createRoom(dto);

    obs.subscribe({
      next: () => {
        this.notify.success(this.editingRoom() ? 'Room updated successfully.' : 'Room created successfully.');
        this.closeModal(); this.loadRooms(); this.isSubmitting.set(false);
      },
      error: err => {
        this.notify.error(err?.error?.message || 'Operation failed.');
        this.isSubmitting.set(false);
      }
    });
  }

  confirmDelete(id: number): void { this.deleteConfirmId.set(id); }
  cancelDelete(): void { this.deleteConfirmId.set(null); }

  deleteRoom(id: number): void {
    this.roomService.deleteRoom(id).subscribe({
      next: () => { this.notify.success('Room deleted.'); this.loadRooms(); this.deleteConfirmId.set(null); },
      error: () => this.notify.error('Delete failed.')
    });
  }

  getStatusClass(s: string): string {
    return s === 'Available' ? 'badge-success' : s === 'Occupied' ? 'badge-error' : 'badge-warning';
  }
}
