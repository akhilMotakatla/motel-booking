import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { RoomService } from '../../../core/services/room.service';
import { Room } from '../../../core/models/room.model';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-room-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './room-detail.component.html',
  styleUrls: ['./room-detail.component.scss']
})
export class RoomDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private roomService = inject(RoomService);
  public authService = inject(AuthService);

  room = signal<Room | null>(null);
  isLoading = signal(true);
  activeImage = signal(0);
  activeTab = signal<'overview'|'amenities'|'reviews'>('overview');

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.roomService.getRoom(id).subscribe({
      next: r => { this.room.set(r); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  setImage(i: number): void { this.activeImage.set(i); }
  nextImage(): void {
    const imgs = this.room()?.imageUrls ?? [];
    this.activeImage.set((this.activeImage() + 1) % imgs.length);
  }
  prevImage(): void {
    const imgs = this.room()?.imageUrls ?? [];
    this.activeImage.set((this.activeImage() - 1 + imgs.length) % imgs.length);
  }

  getStars(r: number): number[] { return Array(Math.round(r)).fill(0); }
  getEmptyStars(r: number): number[] { return Array(5 - Math.round(r)).fill(0); }

  getStatusClass(s: string): string {
    return s === 'Available' ? 'badge-success' : s === 'Occupied' ? 'badge-error' : 'badge-warning';
  }

  get currentImage(): string {
    const r = this.room();
    if (!r) return '';
    return r.imageUrls[this.activeImage()] || r.thumbnailUrl || '';
  }

  get amenityGroups(): { category: string; items: any[] }[] {
    const r = this.room();
    if (!r) return [];
    const groups: Record<string, any[]> = {};
    r.amenities.forEach(a => {
      const cat = a.category || 'General';
      if (!groups[cat]) groups[cat] = [];
      groups[cat].push(a);
    });
    return Object.entries(groups).map(([category, items]) => ({ category, items }));
  }
}
